namespace CorpFleet.Events;

public class VehicleEventArgs : EventArgs
{
    public string Message { get; }
    public double OperationPrice { get; }

    public VehicleEventArgs(string message, double operationPrice)
    {
        Message = message;
        OperationPrice = operationPrice;
    }
}
