package ch.hslu.sw02;
import java.lang.Math;

public class Temperature {
    private float degreeCelsius = 0;

    
    /** 
     * @return float temperature in degree celsius
     */
    public float getDegreeCelsius(){
        return this.degreeCelsius;
    }
    
    
    /** 
     * @return float temperature in kelvin
     */
    public float getKelvin(){
        return (this.degreeCelsius + 273.15f);
    }

    
    /** 
     * @return int temperature in Fahrenheit
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
        this.degreeCelsius = Math.round(this.degreeCelsius*100)/100f;
    }

    
    /** 
     * @param temp
     */
    public void setFahrenheit(float temp){
        this.degreeCelsius = temp / 1.8f - 32;
    }

    
    /** 
     * @param temp
     */
    public void addKelvin(float temp){
        this.degreeCelsius += temp;
    }

    Temperature(){}

    Temperature(float tempValue, String tempType) throws Exception{
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
