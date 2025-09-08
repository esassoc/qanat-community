using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Stateless;
using Stateless.Graph;

namespace Qanat.EFModels.Workflows;

public class WellRegistryWorkflow : IWorkflow
{
    public enum Trigger { 
        Create, 
        UpdateSelectedParcel,
        UpdateWellLocation, 
        ConfirmWellLocation,
        UpdateWellIrrigatedParcels,
        UpdateWellContacts,
        UpdateBasicInfo,
        UpdateSupportingInfo,
        AddAttachment,
        UpdateAttachment,
        Submit, 
        Return, 
        Approve,
        Delete
    }

    private readonly StateMachine<WellRegistrationStatusEnum, Trigger> _machine;

    // The TriggerWithParameters object is used when a trigger requires a payload.
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<int?> _updateSelectedParcelTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<WellRegistrationLocationDto> _updateWellLocationTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<ConfirmWellRegistrationLocationDto> _confirmWellLocationTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<WellRegistrationIrrigatedParcelsRequestDto> _updateWellIrrigatedParcelsTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<WellRegistrationContactsUpsertDto> _updateWellContactsTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<WellRegistrationBasicInfoFormDto> _updateBasicInfoTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<WellRegistrySupportingInfoDto> _updateSupportingInfoTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<WellRegistrationFileResourceUpsertDto, FileResource> _addAttachmentTrigger;
    private readonly StateMachine<WellRegistrationStatusEnum, Trigger>.TriggerWithParameters<WellRegistrationFileResourceUpdateDto> _updateAttachmentTrigger;

    private WellRegistration _wellRegistration;
    private UserDto _currentUser;
    private readonly QanatDbContext _dbContext;


    /// <summary>
    /// Constructor for the WellRegistryWorkflow class
    /// </summary>
    /// <param name="wellRegistration">The well for the workflow</param>
    public WellRegistryWorkflow(QanatDbContext dbContext, WellRegistration wellRegistration, UserDto currentUser)
    {
        _dbContext = dbContext;
        _wellRegistration = wellRegistration;
        _currentUser = currentUser;

        // Instantiate a new state machine in the Open state
        _machine = new StateMachine<WellRegistrationStatusEnum, Trigger>(_wellRegistration.WellRegistrationStatus.ToEnum);

        // Instantiate triggers that have parameters
        _updateSelectedParcelTrigger = _machine.SetTriggerParameters<int?>(Trigger.UpdateSelectedParcel);
        _updateWellLocationTrigger = _machine.SetTriggerParameters<WellRegistrationLocationDto>(Trigger.UpdateWellLocation);
        _confirmWellLocationTrigger = _machine.SetTriggerParameters<ConfirmWellRegistrationLocationDto>(Trigger.ConfirmWellLocation);
        _updateWellIrrigatedParcelsTrigger = _machine.SetTriggerParameters<WellRegistrationIrrigatedParcelsRequestDto>(Trigger.UpdateWellIrrigatedParcels);
        _updateWellContactsTrigger = _machine.SetTriggerParameters<WellRegistrationContactsUpsertDto>(Trigger.UpdateWellContacts);
        _updateBasicInfoTrigger = _machine.SetTriggerParameters<WellRegistrationBasicInfoFormDto>(Trigger.UpdateBasicInfo);
        _updateSupportingInfoTrigger = _machine.SetTriggerParameters<WellRegistrySupportingInfoDto>(Trigger.UpdateSupportingInfo);
        _addAttachmentTrigger = _machine.SetTriggerParameters<WellRegistrationFileResourceUpsertDto, FileResource>(Trigger.AddAttachment);
        _updateAttachmentTrigger = _machine.SetTriggerParameters<WellRegistrationFileResourceUpdateDto>(Trigger.UpdateAttachment);
        
         
        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    { 
        // Configure the Draft state
        var draftState = _machine.Configure(WellRegistrationStatusEnum.Draft)
            .InternalTransition(Trigger.Create, (t) => WellRegistryWorkflowActions.CreateWellRegistration(_dbContext, _wellRegistration))
            .PermitIf(Trigger.Submit, WellRegistrationStatusEnum.Submitted, () => WellRegistryWorkflowProgress.CanSubmit(_dbContext,_wellRegistration));
        AddInternalUpdateTransitions(draftState);

        // Configure the Returned state
        var returnedState = _machine.Configure(WellRegistrationStatusEnum.Returned)
            .OnEntryFrom(Trigger.Return, OnReturned)
            .Permit(Trigger.Submit, WellRegistrationStatusEnum.Submitted);
        AddInternalUpdateTransitions(returnedState);

        // Configure the Submitted state
        var submittedState = _machine.Configure(WellRegistrationStatusEnum.Submitted)
            .OnEntryFrom(Trigger.Submit, OnSubmitted)  // This is where the TriggerWithParameters is used. Note that the TriggerWithParameters object is used, not something from the enum
            .Permit(Trigger.Return, WellRegistrationStatusEnum.Returned)
            .Permit(Trigger.Approve, WellRegistrationStatusEnum.Approved);
        AddInternalUpdateTransitions(submittedState);

        // Configure the Approved state
        var approvedState = _machine.Configure(WellRegistrationStatusEnum.Approved)
            .InternalTransitionIf(Trigger.Delete, (t) => WellRegistryWorkflowProgress.CanDelete(_dbContext, _wellRegistration, _currentUser), (t) => WellRegistryWorkflowActions.DeleteWellRegistry(_dbContext, _wellRegistration))
            .OnEntryFrom(Trigger.Approve, OnApproved);
        AddInternalUpdateTransitions(approvedState);
    }

    private void AddInternalUpdateTransitions(StateMachine<WellRegistrationStatusEnum, Trigger>.StateConfiguration stateConfiguration)
    {
        stateConfiguration
            .InternalTransition(_updateSelectedParcelTrigger, (parcelID, t) => WellRegistryWorkflowActions.UpdateSelectedParcel(_dbContext, _wellRegistration, parcelID))
            .InternalTransition(_updateWellLocationTrigger, (dto, t) => WellRegistryWorkflowActions.UpdateWellRegistrationLocation(_dbContext, _wellRegistration, dto))
            .InternalTransition(_confirmWellLocationTrigger, (dto, t) => WellRegistryWorkflowActions.ConfirmWellRegistrationLocation(_dbContext, _wellRegistration, dto))
            .InternalTransition(_updateWellIrrigatedParcelsTrigger, (dto, t) => WellRegistryWorkflowActions.UpdateWellRegistrationIrrigatedParcels(_dbContext, _wellRegistration, dto))
            .InternalTransition(_updateWellContactsTrigger, (dto, t) => WellRegistryWorkflowActions.UpsertWellContacts(_dbContext, _wellRegistration, dto))
            .InternalTransition(_updateBasicInfoTrigger, (dto, t) => WellRegistryWorkflowActions.UpdateWellBasicInfo(_dbContext, _wellRegistration, dto))
            .InternalTransition(_updateSupportingInfoTrigger, (dto, t) => WellRegistryWorkflowActions.UpdateWellRegistrationSupportingInfo(_dbContext, _wellRegistration, dto))
            .InternalTransition(_addAttachmentTrigger, (dto, fr, t) => WellRegistryWorkflowActions.AddAttachment(_dbContext, _wellRegistration, dto, fr))
            .InternalTransition(_updateAttachmentTrigger, (dto, t) => WellRegistryWorkflowActions.UpdateAttachment(_dbContext, _wellRegistration, dto))
            .InternalTransitionIf(Trigger.Delete, (t) => WellRegistryWorkflowProgress.CanDelete(_dbContext, _wellRegistration, _currentUser), (t) => WellRegistryWorkflowActions.DeleteWellRegistry(_dbContext, _wellRegistration));
    }

    public WellRegistryWorkflowProgress.WellRegistryWorkflowProgressDto GetProgressDto()
    {
        return WellRegistryWorkflowProgress.GetProgress(_wellRegistration);
    }

    public bool CanFireTrigger(Trigger trigger)
    {
        return _machine.CanFire(trigger);
    }

    public void Create()
    {
        _machine.Fire(Trigger.Create);
    }

    public void Submit()
    {
        _machine.Fire(Trigger.Submit);
    }

    public void Approve()
    {
        _machine.Fire(Trigger.Approve);
    }

    public void Delete()
    {
        _machine.Fire(Trigger.Delete);
    }

    public void Return()
    {
        _machine.Fire(Trigger.Return);
    }
    public void UpdateSelectedParcel(int? parcelID)
    {
        _machine.Fire(_updateSelectedParcelTrigger, parcelID);
    }
    public void UpdateWellRegistrationLocation(WellRegistrationLocationDto dto)
    {
        _machine.Fire(_updateWellLocationTrigger, dto);
    }

    public void ConfirmWellRegistrationLocation(ConfirmWellRegistrationLocationDto dto)
    {
        _machine.Fire(_confirmWellLocationTrigger, dto);
    }

    public void UpdateIrrigatedParcels(WellRegistrationIrrigatedParcelsRequestDto dto)
    {
        _machine.Fire(_updateWellIrrigatedParcelsTrigger, dto);
    }

    public void UpdateWellRegistrationContacts(WellRegistrationContactsUpsertDto dto)
    {
        _machine.Fire(_updateWellContactsTrigger, dto);
    }

    public void UpdateBasicInfo(WellRegistrationBasicInfoFormDto formDto)
    {
        _machine.Fire(_updateBasicInfoTrigger, formDto);
    }

    public void UpdateSupportingInfo(WellRegistrySupportingInfoDto dto)
    {
        _machine.Fire(_updateSupportingInfoTrigger, dto);
    }

    public void AddAttachment(WellRegistrationFileResourceUpsertDto dto, FileResource fileResource)
    {
        _machine.Fire(_addAttachmentTrigger, dto, fileResource);
    }

    public void UpdateAttachment(WellRegistrationFileResourceUpdateDto dto)
    {
        _machine.Fire(_updateAttachmentTrigger, dto);
    }

    public int GetWellRegistrationID()
    {
        return _wellRegistration.WellRegistrationID;
    }

    /// <summary>
    /// This method is called automatically when the Assigned state is entered, but only when the trigger is _assignTrigger.
    /// </summary>
    /// <param name="assignee"></param>
    private void OnApproved()
    {
        WellRegistryWorkflowActions.ApproveWellRegistration(_dbContext, _wellRegistration);
    }

    private void OnReturned()
    {
        WellRegistryWorkflowActions.ReturnWell(_dbContext, _wellRegistration);
    }

    private void OnSubmitted()
    {
        WellRegistryWorkflowActions.SubmitWell(_dbContext,_wellRegistration);
    }

    private void SendEmailToManager(string message)
    {
        Console.WriteLine(message);
    }

    private void SendEmailToOwner(string message)
    {
        Console.WriteLine(message);
    }

    public string ToDotGraph()
    {
        return UmlDotGraph.Format(_machine.GetInfo());
    }

}
