using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SeaProject
{
    public class FAS_Element : INotifyPropertyChanged
    {
        private string name;
        private string cname;
        private string extrainfo;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("name");
            }
        }
        public string Cname
        {
            get { return cname; }
            set
            {
                cname = value;
                RaisePropertyChanged("name");
            }
        }
        public string Extrainfo
        {
            get { return extrainfo; }
            set
            {
                extrainfo = value;
                RaisePropertyChanged("name");
            }
        }
        #region Unused prop
        public string Mainentrystructureid { get; set; }
        public string Parententryid { get; set; }
        public string Entryid { get; set; }
        public string Entryclassid { get; set; }
        public string Productid { get; set; }
        public string Materialid { get; set; }
        public string Departmentid { get; set; }
        public string Classcode { get; set; }
        public string Shipnumber { get; set; }
        public string Decimalcode { get; set; }
        public string Maindocument { get; set; }
        public string Normresource { get; set; }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<FAS_Element> FAS_ElementList { get; set; }
    }
    /*  select mainentrystructureid, parententryid, prj_entry.entryid, entryclassid, productid, materialid, departmentid, name, cname, classcode, shipnumber, decimalcode, maindocument, extrainfo, normresource 
        from lnk_mainentrystructure 
        RIGHT JOIN public.prj_entry 
        ON lnk_mainentrystructure.entryid=prj_entry.entryid;
    */
}
