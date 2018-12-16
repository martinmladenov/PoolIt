namespace PoolIt.Infrastructure
{
    public static class NotificationMessages
    {
        public const string RegistrationWelcome = "Welcome to PoolIt, {0}!";
        public const string LoggedOut = "Logged out successfully";
        public const string PasswordChanged = "Password changed successfully";
        public const string PasswordSet = "Password set successfully";
        public const string ProfileDetailsUpdated = "Details updated successfully";

        public const string CarCreateError = "An error occured while adding your car, try again later";
        public const string CarCreated = "Your car was added, you can now organise a ride";
        public const string CarEdited = "Car information updated";
        public const string CarEditError = "An error occured while updating car information";
        public const string CarDeleted = "Car deleted";
        public const string CarDeleteError = "An error occured while deleting this car";

        public const string ModelCreated = "Model added";

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