namespace OpenMedStack;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public class TopicAttribute : Attribute
{
    public TopicAttribute(string topic)
    {
        Topic = topic;
    }
    
    public string Topic { get; }
}