public interface IAnimatorSync
{
    void SetTrigger(string triggerName);
    void SetParameter(string name, float val);
    void SetParameter(string name, int val);
    void SetParameter(string name, bool val);
}