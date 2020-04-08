namespace GroupDocs.Total.MVC.Products.Viewer.Cache
{
    interface IKeyLockerStore
    {
        object GetLockerFor(string key);
    }
}
