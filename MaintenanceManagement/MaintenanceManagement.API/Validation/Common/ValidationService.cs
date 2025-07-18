using Core.Application.MachineGroup.Command.CreateMachineGroup;
using Core.Application.MachineGroup.Command.UpdateMachineGroup;
using Core.Application.MiscMaster.Command.CreateMiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using Core.Application.CostCenter.Command.CreateCostCenter;
using Core.Application.CostCenter.Command.DeleteCostCenter;
using Core.Application.CostCenter.Command.UpdateCostCenter;
using Core.Application.WorkCenter.Command.CreateWorkCenter;
using Core.Application.WorkCenter.Command.DeleteWorkCenter;
using Core.Application.WorkCenter.Command.UpdateWorkCenter;



using Core.Application.ShiftMasters.Commands.CreateShiftMaster;
using Core.Application.ShiftMasters.Commands.DeleteShiftMaster;
using Core.Application.ShiftMasters.Commands.UpdateShiftMaster;
using Core.Application.ShiftMasterDetails.Commands.CreateShiftMasterDetail;
using Core.Application.ShiftMasterDetails.Commands.DeleteShiftMasterDetail;
using Core.Application.ShiftMasterDetails.Commands.UpdateShiftMasterDetail;
using FluentValidation;
using MaintenanceManagement.API.Validation.CostCenter;
using MaintenanceManagement.API.Validation.WorkCenter;
using MaintenanceManagement.API.Validation.MachineGroup;
using MaintenanceManagement.API.Validation.MiscMaster;
using MaintenanceManagement.API.Validation.MiscTypeMaster;
using MaintenanceManagement.API.Validation.ShiftMaster;
using MaintenanceManagement.API.Validation.ShiftMasterDetail;
using Core.Application.MaintenanceCategory.Command.CreateMaintenanceCategory;
using MaintenanceManagement.API.Validation.MaintenanceCategory;
using Core.Application.MaintenanceCategory.Command.DeleteMaintenanceCategory;
using Core.Application.MaintenanceCategory.Command.UpdateMaintenanceCategory;
using MaintenanceManagement.API.Validation.MaintenanceType;
using Core.Application.MaintenanceType.Command.CreateMaintenanceType;
using Core.Application.MaintenanceType.Command.DeleteMaintenanceType;
using Core.Application.MaintenanceType.Command.UpdateMaintenanceType;
using Core.Application.MachineGroup.Command.DeleteMachineGroup;
using Core.Application.MiscMaster.Command.DeleteMiscMaster;
using Core.Application.MiscTypeMaster.Command.DeleteMiscTypeMaster;
using Core.Application.ActivityMaster.Command.CreateActivityMaster;
using MaintenanceManagement.API.Validation.ActivityMaster;
using MaintenanceManagement.API.Validation.MachineMaster;
using Core.Application.ActivityMaster.Command.UpdateActivityMster;
using Core.Application.MachineMaster.Command.CreateMachineMaster;
using MaintenanceManagement.API.Validation.MachineGroupUser;
using Core.Application.MachineMaster.Command.UpdateMachineMaster;
using Core.Application.MachineGroupUsers.Command.CreateMachineGroupUser;
using Core.Application.MachineMaster.Command.DeleteMachineMaster;
using Core.Application.MachineGroupUser.Command.UpdateMachineGroupUser;
using Core.Application.MachineGroupUser.Command.DeleteMachineGroupUser;

using Core.Application.ActivityCheckListMaster.Command.CreateActivityCheckListMaster;
using MaintenanceManagement.API.Validation.ActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Command.UpdateActivityCheckListMaster;
using Core.Application.MaintenanceRequest.Command.CreateMaintenanceRequest;
using MaintenanceManagement.API.Validation.MaintenanceRequest;
using Core.Application.MaintenanceRequest.Command.UpdateMaintenanceRequestCommand;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using MaintenanceManagement.API.Validation.WorkOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder;
using Core.Application.WorkOrder.Command.UploadFileWorOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder.UpdateSchedule;
using Core.Application.WorkOrder.Command.CreateWorkOrder.CreateSchedule;
using Core.Application.WorkOrder.Command.UploadFileWorOrder.Item;
using Core.Application.MRS.Command.CreateMRS;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using MaintenanceManagement.API.Validation.MRS;
using MaintenanceManagement.API.Validation.PreventiveSchedulers;
using Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.DeletePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.ActiveInActivePreventive;
using Core.Application.PreventiveSchedulers.Commands.RescheduleBulkImport;
using Core.Application.Power.FeederGroup.Command.CreateFeederGroup;
using MaintenanceManagement.API.Validation.Power.FeederGroup;
using Core.Application.Power.FeederGroup.Command.UpdateFeederGroup;
using Core.Application.Power.FeederGroup.Command.DeleteFeederGroup;
using Core.Application.Power.PowerConsumption.Command.CreatePowerConsumption;
using MaintenanceManagement.API.Validation.Power.PowerConsumption;
using Core.Application.Power.Feeder.Command.CreateFeeder;
using MaintenanceManagement.API.Validation.Power.Feeder;
using Core.Application.Power.Feeder.Command.UpdateFeeder;
using Core.Application.Power.Feeder.Command.DeleteFeeder;
using Core.Application.ActivityCheckListMaster.Command.DeleteActivityCheckListMaster;
using Core.Application.PreventiveSchedulers.Commands.MapMachine;
using Core.Application.PreventiveSchedulers.Commands.MachineWiseFrequencyUpdate;
using Core.Application.MachineSpecification.Command.CreateMachineSpecfication;
using MaintenanceManagement.API.Validation.MachineSpecification;
using Core.Application.MachineSpecification.DeleteMachineSpecfication;
using MaintenanceManagement.API.Validation.Power.GeneratorConsumption;
using Core.Application.Power.GeneratorConsumption.Command;
using Core.Application.MachineSpecification.Command.UpdateMachineSpecfication;

namespace MaintenanceManagement.API.Validation.Common
{
    public class ValidationService
    {

        public void AddValidationServices(IServiceCollection services)
        {

            services.AddScoped<MaxLengthProvider>();
            services.AddScoped<IValidator<CreateMachineGroupCommand>, CreateMachineGroupCommandValidator>();
            services.AddScoped<IValidator<DeleteMachineGroupCommand>, DeleteMachineGroupCommandValidator>();
            services.AddScoped<IValidator<UpdateMachineGroupCommand>, UpdateMachineGroupCommandValidator>();
            services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscTypeMasterCommand>, DeleteMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMiscMasterCommand>, CreateMiscMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscMasterCommand>, DeleteMiscMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscMasterCommand>, UpdateMiscMasterCommandValidator>();



            services.AddScoped<IValidator<CreateShiftMasterCommand>, CreateShiftMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateShiftMasterCommand>, UpdateShiftMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteShiftMasterCommand>, DeleteShiftMasterCommandValidator>();
            services.AddScoped<IValidator<CreateShiftMasterDetailCommand>, CreateShiftMasterDetailCommandValidator>();
            services.AddScoped<IValidator<UpdateShiftMasterDetailCommand>, UpdateShiftMasterDetailCommandValidator>();
            services.AddScoped<IValidator<DeleteShiftMasterDetailCommand>, DeleteShiftMasterDetailCommandValidator>();
            services.AddScoped<IValidator<CreateCostCenterCommand>, CreateCostCenterCommandValidator>();
            services.AddScoped<IValidator<UpdateCostCenterCommand>, UpdateCostCenterCommandValidator>();
            services.AddScoped<IValidator<DeleteCostCenterCommand>, DeleteCostCenterCommandValidator>();
            services.AddScoped<IValidator<CreateWorkCenterCommand>, CreateWorkCenterCommandValidator>();
            services.AddScoped<IValidator<UpdateWorkCenterCommand>, UpdateWorkCenterCommandValidator>();
            services.AddScoped<IValidator<DeleteWorkCenterCommand>, DeleteWorkCenterCommandValidator>();
            services.AddScoped<IValidator<CreateMaintenanceCategoryCommand>, CreateMaintenanceCategoryCommandValidator>();
            services.AddScoped<IValidator<UpdateMaintenanceCategoryCommand>, UpdateMaintenanceCategoryCommandValidator>();
            services.AddScoped<IValidator<DeleteMaintenanceCategoryCommand>, DeleteMaintenanceCategoryCommandValidator>();
            services.AddScoped<IValidator<CreateMaintenanceTypeCommand>, CreateMaintenanceTypeCommandValidator>();
            services.AddScoped<IValidator<UpdateMaintenanceTypeCommand>, UpdateMaintenanceTypeCommandValidator>();
            services.AddScoped<IValidator<DeleteMaintenanceTypeCommand>, DeleteMaintenanceTypeCommandValidator>();

            services.AddScoped<IValidator<CreateActivityMasterCommand>, CreateActivityMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMachineMasterCommand>, CreateMachineMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMachineMasterCommand>, UpdateMachineMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMachineMasterCommand>, DeleteMachineMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateActivityMasterCommand>, UpdateActivityMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMachineGroupUserCommand>, CreateMachineGroupUserCommandValidator>();
            services.AddScoped<IValidator<UpdateMachineGroupUserCommand>, UpdateMachineGroupUserCommandValidator>();
            services.AddScoped<IValidator<DeleteMachineGroupUserCommand>, DeleteMachineGroupUserCommandValidator>();


            services.AddScoped<IValidator<CreateWorkOrderCommand>, CreateWorkOrderCommandValidator>();
            services.AddScoped<IValidator<UpdateWorkOrderCommand>, UpdateWorkOrderCommandValidator>();
            services.AddScoped<IValidator<UploadFileWorkOrderCommand>, UploadWorkOrderCommandValidator>();
            services.AddScoped<IValidator<UploadFileItemCommand>, UploadItemCommandValidator>();

            services.AddScoped<IValidator<CreateActivityCheckListMasterCommand>, CreateActivityCheckListMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateActivityCheckListMasterCommand>, UpdateActivityCheckListMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteActivityCheckListMasterCommand>, DeleteActivityCheckListMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMaintenanceRequestCommand>, CreateMaintenanceRequestCommandValidator>();
            services.AddScoped<IValidator<UpdateMaintenanceRequestCommand>, UpdateMaintenanceRequestCommandValidator>();
            services.AddScoped<IValidator<UpdateWOScheduleCommand>, UpdateWOScheduleCommandValidator>();


            services.AddScoped<IValidator<CreateWOScheduleCommand>, CreateWOScheduleCommandValidator>();
            services.AddScoped<IValidator<CreateMRSCommand>, CreateMRSCommandValidator>();
            services.AddScoped<IValidator<CreatePreventiveSchedulerCommand>, CreatePreventiveSchedulerCommandValidator>();
            services.AddScoped<IValidator<UpdatePreventiveSchedulerCommand>, UpdatePreventiveSchedulerCommandValidator>();
            services.AddScoped<IValidator<DeletePreventiveSchedulerCommand>, DeletePreventiveSchedulerCommandValidator>();
            services.AddScoped<IValidator<ActiveInActivePreventiveCommand>, UpdateActiveStatusCommandValidator>();
            services.AddScoped<IValidator<RescheduleBulkImportCommand>, BulkImportPreventiveSchedulerCommandValidator>();
            services.AddScoped<IValidator<CreateFeederGroupCommand>, CreateFeederGroupCommandValidator>();
            services.AddScoped<IValidator<UpdateFeederGroupCommand>, UpdateFeederGroupCommandValidator>();

            services.AddScoped<IValidator<DeleteFeederGroupCommand>, DeleteFeederGroupCommandValidator>();
            services.AddScoped<IValidator<CreateFeederCommand>, CreateFeederCommandValidator>();
            services.AddScoped<IValidator<UpdateFeederCommand>, UpdateFeederCommandValidator>();
            services.AddScoped<IValidator<DeleteFeederCommand>, DeleteFeederCommandValidator>();

            services.AddScoped<IValidator<CreatePowerConsumptionCommand>, CreatePowerConsumptionCommandValidator>();
            services.AddScoped<IValidator<CreateGeneratorConsumptionCommand>, CreateGeneratorConsumptionCommandValidator>();

            services.AddScoped<IValidator<MapMachineCommand>, MapMachineCommandValidator>();
            services.AddScoped<IValidator<MachineWiseFrequencyUpdateCommand>, MachineWiseFrequencyUpdateCommandValidator>();
            services.AddScoped<IValidator<CreateMachineSpecficationCommand>, CreateMachineSpecCommandValidator>();
            services.AddScoped<IValidator<DeleteMachineSpecficationCommand>, DeleteMachineSpecCommandValidator>();
            services.AddScoped<IValidator<UpdateMachineSpecficationCommand>, UpdateMachineSpecCommandValidator>();
           



           
        }  
    }
}