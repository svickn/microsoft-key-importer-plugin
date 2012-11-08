using System;
using System.Text;
using System.Threading;
using System.Xml;
using KeePass.DataExchange;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.Util;
using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;

namespace MicrosoftKeyImporterPlugin
{
    class MicrosoftKeysExportFileFormatProvider : FileFormatProvider
    {
        public override bool SupportsImport { get { return true; }}
        public override bool SupportsExport { get { return false; } }
        public override string FormatName { get { return "MSDN or TechNet Keys XML"; }}
        public override string DefaultExtension { get { return "xml"; } }

        public override void Import(PwDatabase pwStorage, System.IO.Stream sInput, IStatusLogger slLogger)
        {
            var document = new XmlDocument();
            document.Load(sInput);

            var root = document.DocumentElement;
            var products = root.SelectNodes("Product_Key");

            if (products == null || products.Count == 0)
                return;

            var msdnGroup = pwStorage.RootGroup.FindCreateGroup("Microsoft Product Keys", true);

            for (int i = 0; i < products.Count; i++ )
            {
                var product = new Product(products[i]);
                slLogger.SetText(string.Format("{0} ({1} of {2})", product.Name, i + 1, products.Count), LogStatusType.Info);
                AddProduct(pwStorage, msdnGroup, product);
            }
        }

        private void AddProduct(PwDatabase database, PwGroup group, Product product)
        {
            var productGroup = group.FindCreateGroup(product.Name, true);

            foreach (var key in product.Keys)
            {
                if(!GroupContainsKeyAsPassword(productGroup,key))
                    AddKey(database, productGroup, key);
            }
        }

        private void AddKey(PwDatabase database, PwGroup group, Key key)
        {
            var entry = new PwEntry(true, true);

            group.AddEntry(entry, true);

            entry.Strings.Set(PwDefs.TitleField, new ProtectedString(database.MemoryProtection.ProtectTitle, key.Type));
            entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(database.MemoryProtection.ProtectPassword, key.Value));
            entry.Strings.Set(PwDefs.NotesField, new ProtectedString(database.MemoryProtection.ProtectNotes, key.Description));
        }

        private bool GroupContainsKeyAsPassword(PwGroup group, Key key)
        {
            foreach (var entry in group.Entries)
            {
                if (key.Value == entry.Strings.Get(PwDefs.PasswordField).ReadString())
                    return true;
            }
            return false;
        }
    }
}
