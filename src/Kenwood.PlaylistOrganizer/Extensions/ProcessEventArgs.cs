using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Extensions
{
    public class ProgressEventArgs : EventArgs
    {
        private float m_progress;

        public ProgressEventArgs(float progress)
        {
            m_progress = progress;
        }

        public float Progress => m_progress;

    }
}
