﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iDna.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>covid-5|http://covid-5.5ync.net/|covid-5ync project home page</string>
  <string>ncbi covid-19 sequence download|https://www.ncbi.nlm.nih.gov/genbank/sars-cov-2-seqs/|ncbi covid-19 genome page</string>
  <string>ncbi sars web page|https://www.ncbi.nlm.nih.gov/genomes/SARS/SARS.html|ncbi sars home page</string>
  <string>covid-19 situation|https://www.cdc.gov/coronavirus/2019-nCoV/summary.html|latest information about covid-19 epidemic</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection UserBookMarks {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["UserBookMarks"]));
            }
            set {
                this["UserBookMarks"] = value;
            }
        }
    }
}
