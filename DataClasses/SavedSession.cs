namespace FreediverApp.DataClasses
{
    public class SavedSession
    {
        public string ref_user;
        public string sessiondate;
        public SavedSession() { }

        public SavedSession(string _ref_user, string _sessiondate)
        {
            ref_user = _ref_user;
            sessiondate = _sessiondate;
        }   
    }
}