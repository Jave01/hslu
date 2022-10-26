package ch.hslu.sw05.chemistry;

public class Temperature {
    private float degreeCelsius = 0;

    public Temperature(){}

    public Temperature(float tempValue, TemperatureType tempType){
        switch (tempType) {
            case KELVIN:
                this.setKelvin(tempValue);
                break;

            case CELSIUS:
                this.setDegreeCelsius(tempValue);
                break;

            case FAHRENHEIT:
                this.setFahrenheit(tempValue);
                break;

            default:
                break;
        }
    }
    
    /** 
     * @return temperature in degree celsius
     */
    public float getDegreeCelsius(){
        return this.degreeCelsius;
    }
    
    
    /** 
     * @return temperature in kelvin.
     */
    public float getKelvin(){
        return (this.degreeCelsius + 273.15f);
    }

    
    /** 
     * @return temperature in Fahrenheit.
     */
    public int getFahrenheit(){
        return (int)(this.degreeCelsius * 1.8f + 32);
    }
    
    
    /** 
     * @param temp new absolute temperature to be set in degree cels  ius.
     */
    public void setDegreeCelsius(float temp){
        this.degreeCelsius = Math.round(temp*100)/100f;
    }

    
    /** 
     * @param temp new absolute temperature in kelvin.
     */
    public void setKelvin(float temp){
        this.degreeCelsius = temp - 273.15f;
        this.degreeCelsius = Math.round(this.degreeCelsius *100)/100f;
    }

    
    /** 
     * @param temp new absolute temperature in degree fahrenheit.
     */
    public void setFahrenheit(float temp){
        this.degreeCelsius = temp / 1.8f - 32;
    }

    /**
     * Get aggregate state from given element at current temperature.
     * @param element element to check
     * @return aggregate state
     */
    public AggregateState getAggregateState(Element element){
        if (this.getKelvin() >= element.getBoilingPoint()){
            return AggregateState.GASEOUS;
        } else if (this.getKelvin() >= element.getMeltingPoint()){
            return AggregateState.FLUID;
        }else {
            return AggregateState.SOLID;
        }
    }

    /**
     * Get aggregate state from a specific element class child.
     * @param element An element which inherited from AbsElement.
     * @return The aggregate state.
     */
    public AggregateState getAggregateState(AbsElement element){
        if (this.getKelvin() >= element.getBoilingPointKelvin()){
            return AggregateState.GASEOUS;
        } else if (this.getKelvin() >= element.getMeltingPointKelvin()){
            return AggregateState.FLUID;
        }else {
            return AggregateState.SOLID;
        }
    }

    /**
     * Increase current temperature by given Kelvin.
     * @param temp New temperature in kelvin.
     */
    public void addKelvin(float temp){
        this.degreeCelsius += temp;
    }
}
