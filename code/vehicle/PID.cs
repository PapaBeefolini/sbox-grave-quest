[System.Serializable]
public class PID 
{
    public float PFactor = 4.0f;
    public float IFactor = 0.0f;
    public float DFactor = 0.1f;
		
	private float integral;
    private float lastError;
	
	public PID(float pFactor, float iFactor, float dFactor) 
	{
		this.PFactor = pFactor;
		this.IFactor = iFactor;
		this.DFactor = dFactor;
	}
	
	public float Update(float setpoint, float actual, float timeFrame) 
	{
		float present = setpoint - actual;
		integral += present * timeFrame;
		float deriv = (present - lastError) / timeFrame;
		lastError = present;
		return present * PFactor + integral * IFactor + deriv * DFactor;
	}
}