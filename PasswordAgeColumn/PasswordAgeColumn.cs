using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util.Spr;
using KeePassLib;

namespace PasswordAgeColumn
{
    public sealed class PasswordAgeColumnExt : Plugin
    {
        private IPluginHost m_host = null;
        private PasswordAgeColumnProv m_prov = null;

        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            m_host = host;

            m_prov = new PasswordAgeColumnProv();
            m_host.ColumnProviderPool.Add(m_prov);


            return true;
        }

        public override void Terminate()
        {
            if (m_host == null) return;

            m_host.ColumnProviderPool.Remove(m_prov);
            m_prov = null;

            m_host = null;
        }

        public override string UpdateUrl
        {
            get
            {
                return "https://raw.githubusercontent.com/tiuub/PasswordAgeColumn/master/VERSION";
            }
        }
    }

    public sealed class PasswordAgeColumnProv : ColumnProvider
    {
        private const string PasswordAgeColumnName = "Password Age";
        private const string PasswordAgeColumnSuffix = " days";

        public override string[] ColumnNames
        {
            get { return new string[] { PasswordAgeColumnName }; }
        }


        public override string GetCellData(string strColumnName, PwEntry pe)
        {
            DateTime dateLastModified = pe.LastModificationTime;
            string strPassword = pe.Strings.ReadSafe(PwDefs.PasswordField);
            int iPasswordAge = DateTime.Now.Subtract(dateLastModified).Days;
            foreach (PwEntry tmpPe in pe.History.ToArray().Reverse())
            {
                if(tmpPe.Strings.ReadSafe(PwDefs.PasswordField) == strPassword)
                {
                    dateLastModified = tmpPe.LastModificationTime;
                    iPasswordAge = DateTime.Now.Subtract(dateLastModified).Days;
                }
                else
                {
                    break;
                }
            }

            return iPasswordAge + PasswordAgeColumnSuffix;
        }
    }
}
