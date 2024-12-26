using MediatR;


namespace BSOFT.Application.Modules.Commands.CreateModule
{
    public class CreateModuleCommand  : IRequest<int>
    {
    public string ModuleName { get; set; }
    public List<string> Menus { get; set; }
    }
}