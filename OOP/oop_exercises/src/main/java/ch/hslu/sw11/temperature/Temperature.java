package ch.hslu.sw11.temperature;

import ch.hslu.sw05.chemistry.AbsElement;
import ch.hslu.sw05.chemistry.AggregateState;
import ch.hslu.sw05.chemistry.Element;
import ch.hslu.sw05.chemistry.TemperatureType;
import ch.hslu.sw09.CustomTemperatureException;

import java.util.Objects;

/**
 * Class for storing and calculating temperatures and aggregate states.
 */
public final class Temperature implements Comparable<Temperature> {
    private float degreeCelsius = 0;

    public static final float kelvinOffset = 273.15f;

    private Temperature(final float tempValue, final TemperatureType tempType){
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

    public static Temperature createFromCelsius(final float celsius){
        return new Temperature(celsius, TemperatureType.CELSIUS);
    }

    public static Temperature createFromKelvin(final float kelvin){
        return new Temperature(kelvin, TemperatureType.KELVIN);
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
    private void setDegreeCelsius(final float temp){
        this.degreeCelsius = Math.round(temp*100)/100f;
    }


    /**
     * @param temp new absolute temperature in kelvin.
     */
    private void setKelvin(final float temp) throws CustomTemperatureException {
        if(temp < 0){
            throw new CustomTemperatureException("Value less than zero");
        }
        this.degreeCelsius = temp - 273.15f;
        this.degreeCelsius = Math.round(this.degreeCelsius *100)/100f;
    }


    /**
     * @param temp new absolute temperature in degree fahrenheit.
     */
    private void setFahrenheit(final float temp){
        this.degreeCelsius = temp / 1.8f - 32;
    }

    /**
     * Get aggregate state from given element at current temperature.
     * @param element element to check
     * @return aggregate state
     */
    public AggregateState getAggregateState(final Element element){
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
    public AggregateState getAggregateState(final AbsElement element){
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

    @Override
    public int compareTo(Temperature other) {
        return Float.compare(this.degreeCelsius, other.degreeCelsius);
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Temperature other)) return false;
        return Float.compare(other.degreeCelsius, this.degreeCelsius) == 0;
    }

    @Override
    public int hashCode() {
        return Objects.hash(this.degreeCelsius);
    }

    @Override
    public String toString(){
        return "Temperatur: " + this.degreeCelsius + "C";
    }
}
