namespace PoolIt.Infrastructure
{
    public static class NotificationMessages
    {
        public const string RegistrationWelcome = "Welcome to PoolIt, {0}!";
        public const string LoggedOut = "Logged out successfully";
        public const string PasswordChanged = "Password changed successfully";
        public const string PasswordSet = "Password set successfully";
        public const string InvalidPassword = "Invalid password";
        public const string ProfileDetailsUpdated = "Details updated successfully";
        public const string AccountDeleted = "We're sorry to see you go. Your account was deleted.";

        public const string AccountDeleteError =
            "An error occured while deleting your account. Try again or contact support.";

        public const string CarCreateError = "An error occured while adding your car, try again later";
        public const string CarCreated = "Your car was added, you can now organise a ride";
        public const string CarEdited = "Car information updated";
        public const string CarEditError = "An error occured while updating car information";
        public const string CarDeleted = "Car deleted";
        public const string CarDeleteError = "An error occured while deleting this car";

        public const string ManufacturerCreated = "Manufacturer added";
        public const string ManufacturerInvalidName = "Manufacturer name must be between 3 and 50 characters long";
        public const string ManufacturerExists = "This manufacturer already exists";
        public const string ManufacturerCreateError = "An error occured while adding manufacturer";
        public const string ManufacturerDeleted = "Manufacturer deleted";
        public const string ManufacturerDeleteError = "An error occured while deleting manufacturer";
        public const string ManufacturerEdited = "Manufacturer information updated";
        public const string ManufacturerEditError = "An error occured while updating manufacturer information";

        public const string ModelCreated = "Model added";
        public const string ModelInvalidName = "Model name must be between 3 and 50 characters long";
        public const string ModelExists = "This model already exists";
        public const string ModelExistsNotCreated = "This model already exists, no need to add it";
        public const string ModelCreateError = "An error occured while adding model";
        public const string ModelDeleted = "Model deleted";
        public const string ModelDeleteError = "An error occured while deleting model";
        public const string ModelEdited = "Model information updated";
        public const string ModelEditError = "An error occured while updating model information";

        public const string InvitationGenerated = "Invitation generated successfully";
        public const string InvitationAccepted = "Invitation accepted";
        public const string InvitationAcceptNoPermission = "You cannot accept this invitation";
        public const string InvitationDeleted = "Invitation deleted";
        public const string InvitationDeleteError = "An error occured while deleting this invitation";

        public const string JoinRequestCreated = "Join request sent to organiser";
        public const string JoinRequestCreateError = "An error occured while sending join request, try again later";
        public const string JoinRequestAccepted = "Join request accepted";
        public const string JoinRequestAcceptError = "An error occured while accepting join request, try again later";
        public const string JoinRequestRefused = "Join request deleted";
        public const string JoinRequestRefuseError = "An error occured while deleting join request, try again later";

        public const string RideCreated = "Ride created";
        public const string RideCreateError = "An error occured while creating ride";
        public const string RideEdited = "Ride information updated";
        public const string RideEditError = "An error occured while updating ride information";
        public const string RideDeleted = "Ride deleted";
        public const string RideDeleteError = "An error occured while deleting ride, try again later";
    }
}