using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace FaultDetectorDotNet.Extension
{
    public class SolutionEventsHandler : IVsUpdateSolutionEvents
    {
        private readonly IVsSolutionBuildManager2 _solutionBuildManager;
        private uint _cookie;

        public SolutionEventsHandler(IVsSolutionBuildManager2 solutionBuildManager)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _solutionBuildManager = solutionBuildManager;
            _solutionBuildManager.AdviseUpdateSolutionEvents(this, out _cookie);
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            BuildStatusChanged?.Invoke(this, true);
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            BuildStatusChanged?.Invoke(this, false);
            return VSConstants.S_OK;
        }

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            BuildStatusChanged?.Invoke(this, true);
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Cancel()
        {
            BuildStatusChanged?.Invoke(this, false);
            return VSConstants.S_OK;
        }

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.S_OK;
        }

        public event EventHandler<bool> BuildStatusChanged;

        public void Unregister()
        {
            if (_solutionBuildManager != null && _cookie != 0)
            {
                _solutionBuildManager.UnadviseUpdateSolutionEvents(_cookie);
                _cookie = 0;
            }
        }
    }
}