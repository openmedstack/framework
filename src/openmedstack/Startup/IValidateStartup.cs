namespace OpenMedStack.Startup;

using System;
using System.Threading.Tasks;

public interface IValidateStartup
{
    Task<Exception?> Validate();
}