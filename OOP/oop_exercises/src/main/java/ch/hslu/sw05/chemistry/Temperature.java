package ch.hslu.sw05.chemistry;

public class Temperature {
    private float degreeCelsius = 0;

    
    /** 
     * @return float
     */
    public float getDegreeCelsius(){
        return this.degreeCelsius;
    }
    
    
    /** 
     * @return float
     */
    public float getKelvin(){
        return (this.degreeCelsius + 273.15f);
    }

    
    /** 
     * @return int
     */
    public int getFahrenheit(){
        return 0;
    }
    
    
    /** 
     * @param temp
     */
    public void setDegreeCelsius(float temp){
        this.degreeCelsius = Math.round(temp*100)/100f;
    }

    
    /** 
     * @param temp
     */
    public void setKelvin(float temp){
        this.degreeCelsius = temp - 273.15f;
        this.degreeCelsius = Math.round(this.degreeCelsius *100)/100f;
    }

    
    /** 
     * @param temp
     */
    public void setFahrenheit(float temp){
        this.degreeCelsius = temp / 1.8f - 32;
    }

    /**
     * get aggregate state from given element at current temperature
     * @param element element to check
     * @return state
     */
    public String getAggregateState(Element element){
        if (this.getKelvin() >= element.getBoilingPoint()){
            return "gaseous";
        } else if (this.getKelvin() >= element.getMeltingPoint()){
            return "fluid";
        }else {
            return "solid";
        }
    }

    public String getAggregateState(AbsElement element){
        if (this.getKelvin() >= element.getBoilingPointKelvin()){
            return "gaseous";
        } else if (this.getKelvin() >= element.getMeltingPointKelvin()){
            return "fluid";
        }else {
            return "solid";
        }
    }

    /** 
     * @param temp
     */
    public void addKelvin(float temp){
        this.degreeCelsius += temp;
    }

    public Temperature(){}

    public Temperature(float tempValue, String tempType){
        switch (tempType.toLowerCase()) {
            case "kelvin":
                this.setKelvin(tempValue);
                break;
        
            case "celsius":
                this.setDegreeCelsius(tempValue);
                break;
        
            case "fahrenheit":
                this.setFahrenheit(tempValue);
                break;
        
            default:
                break;
        }
    }


}
