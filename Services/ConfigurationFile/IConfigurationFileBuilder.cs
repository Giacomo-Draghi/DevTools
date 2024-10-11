using DevTools.MVVM.Model;

namespace DevTools.Services.ConfigurationFile
{
    public interface IConfigurationFileBuilder
    {
        public void CreaCfg(DevToolModel devToolModel);
        public void SetUpConnection(DevToolModel devToolModel);
    }
}
