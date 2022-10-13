package SW2;
import java.lang.Math;

public class Temperature {
    private float degreeCelcius = 0;

    public float getDegreeCelcius(){
        return this.degreeCelcius;
    }
    
    public float getKelvin(){
        return (this.degreeCelcius + 273.15f);
    }

    public void getFahrenheit(){
        return 
    }
    
    public void setDegreeCelcius(float temp){
        this.degreeCelcius = Math.round(temp*100)/100f;
    }

    public void setKelvin(float temp){
        this.degreeCelcius = temp - 273.15f;
        this.degreeCelcius = Math.round(this.degreeCelcius*100)/100f;
    }

    public void setFahrenheit(float temp){
        this.degreeCelcius = temp / 1.8f - 32;
    }

    public void addKelvin(float temp){
        this.degreeCelcius += temp;
    }

    Temperature(){}

    Temperature(float kelvin, float degreeC, float degreeF){
        this.setKelvin(kelvin);
        this.setDegreeCelcius(degreeC);
        this.setFahrenheit(degreeF);
    }
}
