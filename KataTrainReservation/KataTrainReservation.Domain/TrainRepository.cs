namespace KataTrainReservation.Domain
{
    public interface TrainRepository
    {
        Train Get(string trainId);
    }
}