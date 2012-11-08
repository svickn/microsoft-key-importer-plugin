using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using KeePass.Plugins;

namespace MicrosoftKeyImporterPlugin
{
    public sealed class MicrosoftKeyImporterPluginExt : Plugin
    {
        private IPluginHost _host;
        private MicrosoftKeysExportFileFormatProvider _provider;

        public override bool Initialize(IPluginHost host)
        {
            _host = host;
            _provider = new MicrosoftKeysExportFileFormatProvider();

            _host.FileFormatPool.Add(_provider);

            return true;
        }

        public override void Terminate()
        {
            _host.FileFormatPool.Remove(_provider);
        }
    }
}
