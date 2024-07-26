using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EnvDTE;
using EnvDTE80;
using FaultDetectorDotNet.Core.Helpers;
using FaultDetectorDotNet.Core.Suspiciousness;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace FaultDetectorDotNet.Extension
{
    public partial class ToolWindowControl : UserControl, INotifyPropertyChanged
    {
        private readonly SpectrumBasedFaultLocalizationRunner _spectrumBasedFaultLocalizationRunner;

        private readonly Predicate<object> FilterResultsWithScoreGreaterThanZero = item =>
        {
            var resultItem = item as SuspiciousnessItem;
            return resultItem != null && resultItem.Score > 0;
        };

        private ICollectionView _collectionView;
        private DTE2 _dte;
        private SolutionEvents _solutionEvents;
        private SolutionEventsHandler _solutionEventsHandler;
        private CancellationTokenSource _suspiciousnessServiceExecutionCancellationTokenSource;
        private int _totalCount;
        private int _totalWithScoreCount;
        private IProjectHelper _projectHelper;

        public ToolWindowControl()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += ToolWindow1Control_Loaded;
            Unloaded += ToolWindow1Control_Unloaded;
            _spectrumBasedFaultLocalizationRunner = new SpectrumBasedFaultLocalizationRunner();
            SuspiciousnessResultItems = new ObservableCollection<SuspiciousnessItem>();
            SuspiciousnessAggregatedResultItems = new ObservableCollection<NormalizatedSuspiciousnessItem>();
            GridData = new ObservableCollection<DataGridItem>();
            _projectHelper = new ProjectHelper();
        }

        public ObservableCollection<SuspiciousnessItem> SuspiciousnessResultItems { get; }
        public ObservableCollection<NormalizatedSuspiciousnessItem> SuspiciousnessAggregatedResultItems { get; }
        public ObservableCollection<DataGridItem> GridData { get; }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }

        public int TotalWithScoreCount
        {
            get => _totalWithScoreCount;
            set
            {
                _totalWithScoreCount = value;
                OnPropertyChanged(nameof(TotalWithScoreCount));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnBuildStatusChanged(object sender, bool e)
        {
            UpdateControls(e);
        }

        private void ToolWindow1Control_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.AfterClosing += OnSolutionClosed;
            _solutionEvents.Opened += OnSolutionOpened;
            _solutionEvents.ProjectAdded += OnSolutionChanged;
            _solutionEvents.ProjectRemoved += OnSolutionChanged;
            _solutionEvents.ProjectRenamed += OnSolutionRemoved;
            var solutionBuildManager =
                ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
            Assumes.Present(solutionBuildManager);

            _solutionEventsHandler = new SolutionEventsHandler(solutionBuildManager);
            _solutionEventsHandler.BuildStatusChanged += OnBuildStatusChanged;
            CoverageDataGrid.Sorting += CoverageDataGridSortHandler;
            TestProjectsComboBox.SelectionChanged += TestProjectsComboBox_SelectionChanged;
            _spectrumBasedFaultLocalizationRunner.SpectrumBasedFaultLocalizationRunnerStatusChanged +=
                OnSpectrumBasedFaultLocalizationRunnerStatusChanged;
            ShowAllCheckBox.IsChecked = false;

            _collectionView = CollectionViewSource.GetDefaultView(SuspiciousnessResultItems);
            _collectionView.Filter = FilterResultsWithScoreGreaterThanZero;
            SuspiciousnessResultsListView.ItemsSource = SuspiciousnessResultItems;
            NormalizatedSuspiciousnessResultsListView.ItemsSource = SuspiciousnessAggregatedResultItems;
            SuspiciousnessResultItems.CollectionChanged += SuspiciousnessResultItemsCollectionChanged;

            UpdateUi();
        }

        private void SuspiciousnessResultItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SuspiciousnessResultItems.Count == 0)
            {
                SearchTextBox.IsEnabled = false;
            }
            else
            {
                SearchTextBox.IsEnabled = true;
            }

            TotalCount = SuspiciousnessResultItems.Count;
            TotalWithScoreCount =
                SuspiciousnessResultItems.Count(resultItem => !double.IsNaN(resultItem.Score) && resultItem.Score > 0);
        }

        private void OnSpectrumBasedFaultLocalizationRunnerStatusChanged(object sender,
            SpectrumBasedFaultLocalizationRunnerStatusType e)
        {
            var forceDisable = e is SpectrumBasedFaultLocalizationRunnerStatusType.Running;

            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateControls(forceDisable);
                if (e is SpectrumBasedFaultLocalizationRunnerStatusType.Finished)
                {
                    ApplyActionInCheckBoxes(checkBox => checkBox.IsChecked = false,
                        DefaultFaultLocalizationTechniquesPanel, NormalizatedFaultLocalizationTechniquePanel,
                        SymmetryCoefficientPanel);
                    AbortScanButton.Visibility = Visibility.Hidden;
                }
                else if (e is SpectrumBasedFaultLocalizationRunnerStatusType.Running)
                {
                    CoverageDataGrid.Columns.Clear();
                    SuspiciousnessResultItems.Clear();
                    SuspiciousnessAggregatedResultItems.Clear();
                    GridData.Clear();
                    AbortScanButton.Visibility = Visibility.Visible;
                }
            });
        }

        private void TestProjectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateControls();
        }

        private void OnSolutionRemoved(Project project, string oldname)
        {
            LoadTestProjects();
        }

        private void ToolWindow1Control_Unloaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _solutionEvents.AfterClosing -= OnSolutionClosed;
            _solutionEvents.Opened -= OnSolutionOpened;
            _solutionEvents.ProjectAdded -= OnSolutionChanged;
            _solutionEvents.ProjectRemoved -= OnSolutionChanged;
            _solutionEvents.ProjectRenamed -= OnSolutionRemoved;
            _solutionEventsHandler.BuildStatusChanged += OnBuildStatusChanged;
            TestProjectsComboBox.SelectionChanged -= TestProjectsComboBox_SelectionChanged;
            _spectrumBasedFaultLocalizationRunner.SpectrumBasedFaultLocalizationRunnerStatusChanged -=
                OnSpectrumBasedFaultLocalizationRunnerStatusChanged;
            SuspiciousnessResultItems.CollectionChanged -= SuspiciousnessResultItemsCollectionChanged;
            CoverageDataGrid.Sorting -= CoverageDataGridSortHandler;
        }

        private void OnSolutionClosed()
        {
            UpdateUi();
        }

        private void OnSolutionOpened()
        {
            UpdateUi();
        }

        private void OnSolutionChanged(Project project)
        {
            LoadTestProjects();
        }

        private void UpdateControls(bool forceDisable = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (forceDisable)
            {
                TestProjectsComboBox.IsEnabled = false;
                RunButton.IsEnabled = false;
                SearchTextBox.Text = string.Empty;
                ApplyActionInCheckBoxes(checkBox => checkBox.IsEnabled = false, DefaultFaultLocalizationTechniquesPanel,
                    NormalizatedFaultLocalizationTechniquePanel,
                    SymmetryCoefficientPanel);
            }
            else
            {
                var solutionLoaded = _dte.Solution != null && _dte.Solution.IsOpen;
                TestProjectsComboBox.IsEnabled = solutionLoaded && TestProjectsComboBox.SelectedItem != null;
                var anyDefaultFaultLocalizationTechniquesEnabled =
                    GetSelected(DefaultFaultLocalizationTechniquesPanel).Any();
                var normalizatedFaultLocalizationTechniqueEnabled =
                    GetSelected(NormalizatedFaultLocalizationTechniquePanel).Any();
                var anyAdjustableSymmetryCoefficientEnabled = GetSelected(SymmetryCoefficientPanel).Any();
                if (solutionLoaded && TestProjectsComboBox.IsEnabled)
                {
                    ApplyActionInCheckBoxes(checkBox => checkBox.IsEnabled = true,
                        DefaultFaultLocalizationTechniquesPanel, NormalizatedFaultLocalizationTechniquePanel,
                        SymmetryCoefficientPanel);
                }
                else
                {
                    ApplyActionInCheckBoxes(checkBox => checkBox.IsEnabled = false,
                        DefaultFaultLocalizationTechniquesPanel, NormalizatedFaultLocalizationTechniquePanel,
                        SymmetryCoefficientPanel);
                }

                var canEnableScanButton = solutionLoaded
                                          && TestProjectsComboBox.SelectedItem != null
                                          && ((anyDefaultFaultLocalizationTechniquesEnabled &&
                                               anyAdjustableSymmetryCoefficientEnabled)
                                              || (normalizatedFaultLocalizationTechniqueEnabled &&
                                                  !anyDefaultFaultLocalizationTechniquesEnabled));

                RunButton.IsEnabled = canEnableScanButton;
            }
        }

        private void UpdateUi()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            LoadTestProjects();
            UpdateControls();
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }

        private void ClearLog()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            LogTextBox.Clear();
        }

        private void AbortScan_Click(object sender, RoutedEventArgs e)
        {
            if (_suspiciousnessServiceExecutionCancellationTokenSource != null)
            {
                _suspiciousnessServiceExecutionCancellationTokenSource.Cancel();
            }
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var testProjectFullPath = GetSelectedProjectFullPath();
            var selectedTechniques = GetSelected(DefaultFaultLocalizationTechniquesPanel);
            var selectedSymmetryLevels = GetSelected(SymmetryCoefficientPanel);
            var isNormalizatedTechniqueSelected = GetSelected(NormalizatedFaultLocalizationTechniquePanel).Any();

            var processLogger = new TextBoxLoggerAdapter(LogTextBox, true);
            var reporter = new ExtensionReporter(SuspiciousnessResultItems,
                SuspiciousnessAggregatedResultItems,
                GridData,
                CoverageDataGrid);

            using (var cts = new CancellationTokenSource())
            {
                _suspiciousnessServiceExecutionCancellationTokenSource = cts;

                await Task.Run(() =>
                {
                    var parameters = SuspiciousnessServiceParametersFactory.Create(testProjectFullPath,
                        selectedTechniques,
                        selectedSymmetryLevels, isNormalizatedTechniqueSelected);
                    return _spectrumBasedFaultLocalizationRunner.Run(processLogger, reporter, _projectHelper, parameters, cts.Token);
                }, cts.Token);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SuspiciousnessResultItems != null)
            {
                var searchText = SearchTextBox.Text.Trim().ToLower();

                _collectionView.Filter = item =>
                {
                    var resultItem = item as SuspiciousnessItem;
                    if (resultItem == null)
                    {
                        return false;
                    }

                    if (!ShowAllCheckBox.IsChecked.GetValueOrDefault())
                    {
                        if (resultItem.Score == 0 || double.IsNaN(resultItem.Score))
                        {
                            return false;
                        }
                    }

                    return (!string.IsNullOrWhiteSpace(resultItem.Technique) &&
                            resultItem.Technique.ToLower().Contains(searchText)) ||
                           (!string.IsNullOrWhiteSpace(resultItem.AdjustableSymmetryCoefficient) &&
                            resultItem.AdjustableSymmetryCoefficient.ToLower().Contains(searchText)) ||
                           (!string.IsNullOrWhiteSpace(resultItem.Assembly) &&
                            resultItem.Assembly.ToLower().Contains(searchText)) ||
                           (!string.IsNullOrWhiteSpace(resultItem.File) &&
                            resultItem.File.ToLower().Contains(searchText)) ||
                           (!string.IsNullOrWhiteSpace(resultItem.Class) &&
                            resultItem.Class.ToLower().Contains(searchText)) ||
                           (!string.IsNullOrWhiteSpace(resultItem.Method) &&
                            resultItem.Method.ToLower().Contains(searchText)) ||
                           resultItem.Line.ToString().Contains(searchText) || resultItem.Score
                               .ToString(CultureInfo.InvariantCulture).Contains(searchText);
                };

                _collectionView.Refresh();
            }
        }

        private void LoadTestProjects()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            TestProjectsComboBox.Items.Clear();

            if (_dte.Solution == null || !_dte.Solution.IsOpen)
            {
                return;
            }

            var projects = GetAllProjects();
            foreach (var project in projects)
            {
                if (_projectHelper.IsTestProject(project.FileName))
                {
                    TestProjectsComboBox.Items.Add(new ComboBoxItem { Content = project.Name, Tag = project });
                }
            }

            if (TestProjectsComboBox.Items.Count > 0)
            {
                TestProjectsComboBox.SelectedIndex = 0;
            }
        }

        private IEnumerable<Project> GetAllProjects()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var projects = new List<Project>();
            var solutionProjects = _dte.Solution.Projects;

            foreach (Project project in solutionProjects)
            {
                projects.AddRange(GetProjectsRecursive(project));
            }

            return projects;
        }

        private static IEnumerable<Project> GetProjectsRecursive(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var projects = new List<Project>();
            if (project.Kind == EnvDTE.Constants.vsProjectKindSolutionItems)
            {
                for (var i = 1; i <= project.ProjectItems.Count; i++)
                {
                    var subProject = project.ProjectItems.Item(i).SubProject;
                    if (subProject != null)
                    {
                        projects.AddRange(GetProjectsRecursive(subProject));
                    }
                }
            }
            else
            {
                projects.Add(project);
            }

            return projects;
        }

        private string GetSelectedProjectFullPath()
        {
            if (TestProjectsComboBox.SelectedItem == null)
            {
                MessageBox.Show("No project selected.");
                return null;
            }

            var projectName = ((ComboBoxItem)TestProjectsComboBox.SelectedItem).Content.ToString();

            var project = GetAllProjects().FirstOrDefault(p => p.Name == projectName);
            return project?.FullName;
        }

        private void MultiSelector_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object selectedItem = null;
            if (sender is ListView listView && listView.SelectedItem != null)
            {
                selectedItem = listView.SelectedItem;
            }
            else if (sender is DataGrid dataGrid && dataGrid.SelectedItem != null)
            {
                selectedItem = dataGrid.SelectedItem;
            }

            if (selectedItem == null)
            {
                return;
            }

            var fileProperty = selectedItem.GetType().GetProperty("File");
            var lineProperty = selectedItem.GetType().GetProperty("Line");

            if (fileProperty != null && lineProperty != null)
            {
                var filePath = fileProperty.GetValue(selectedItem) as string;
                var lineNumber = lineProperty.GetValue(selectedItem) as int?;
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && lineNumber.HasValue)
                {
                    try
                    {
                        OpenFileInVisualStudio(filePath, lineNumber.Value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening the file in Visual Studio: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid file path or file not found.");
                }
            }
        }

        private static void OpenFileInVisualStudio(string filePath, int lineNumber)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            if (dte != null)
            {
                var window = dte.ItemOperations.OpenFile(filePath);
                window.Visible = true;

                var selection = (TextSelection)dte.ActiveDocument.Selection;
                selection.GotoLine(lineNumber, true);
            }
            else
            {
                MessageBox.Show("Unable to obtain the Visual Studio instance.");
            }
        }

        private void LogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!string.IsNullOrWhiteSpace(LogTextBox.Text))
            {
                ClearLogButton.IsEnabled = true;
            }
            else
            {
                ClearLogButton.IsEnabled = false;
            }
        }

        private void ShowAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _collectionView.Filter = null;
        }

        private void ShowAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _collectionView.Filter = FilterResultsWithScoreGreaterThanZero;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FaultTechniqueCheckBox_Checked_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateControls();
        }

        private static string[] GetSelected(StackPanel stackPanel)
        {
            var selected = new List<string>();

            foreach (var child in stackPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    selected.Add(checkBox.Tag.ToString());
                }
            }

            return selected.ToArray();
        }

        private static void ApplyActionInCheckBoxes(Action<CheckBox> action, params StackPanel[] stackPanels)
        {
            foreach (var stackPanel in stackPanels)
            {
                foreach (var child in stackPanel.Children)
                {
                    if (child is CheckBox checkBox)
                    {
                        action(checkBox);
                    }
                }
            }
        }

        private void SymmetryCoefficientCheckBox_Checked_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateControls();
        }

        private void CoverageDataGridSortHandler(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Line Id")
            {
                var column = e.Column;

                e.Handled = true;

                var direction = column.SortDirection != ListSortDirection.Ascending
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending;

                column.SortDirection = direction;

                var collectionView = CollectionViewSource.GetDefaultView(CoverageDataGrid.ItemsSource);

                if (collectionView is ListCollectionView lcv)
                {
                    lcv.CustomSort = new LineIdComparer(direction);
                }
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}