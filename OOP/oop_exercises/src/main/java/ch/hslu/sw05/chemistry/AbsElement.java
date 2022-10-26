package ch.hslu.sw05.chemistry;

public abstract class AbsElement {
    private float atomicNumber;
    private float boilingPointKelvin;
    private float meltingPointKelvin;

    /**
     * Get atomic number / number of protons from the element.
     * @return int atomic number
     */
    public float getAtomicNumber() {
        return atomicNumber;
    }

    /**
     * Get the boiling point of the element in kelvin.
     * @return int boiling point in kelvin.
     */
    public float getBoilingPointKelvin() {
        return boilingPointKelvin;
    }

    /**
     * Get melting point of the element in kelvin.
     * @return int melting point in kelvin
     */
    public float getMeltingPointKelvin() {
        return meltingPointKelvin;
    }

    /**
     * Get the aggregate state of the element at a given ambient temperature
     * @param tempKelvin current ambient temperature in kelvin
     * @return AggregateState the aggregate state of the element at given temperature.
     */
    public AggregateState getAggregateState(float tempKelvin) {
        if (tempKelvin >= this.boilingPointKelvin){
            return AggregateState.GASEOUS;
        } else if (tempKelvin >= meltingPointKelvin){
            return AggregateState.FLUID;
        } else if (tempKelvin >= 0){
            return AggregateState.SOLID;
        } else {
            return null;
        }
    }

    /**
     * Constructor
     * @param atomicNumber atomic number / number of protons.
     * @param meltingPointKelvin melting point of element in kelvin.
     * @param boilingPointKelvin boiling point of element in kelvin.
     */
    public AbsElement(float atomicNumber, float meltingPointKelvin, float boilingPointKelvin){
        this.atomicNumber = atomicNumber;
        this.meltingPointKelvin = meltingPointKelvin;
        this.boilingPointKelvin = boilingPointKelvin;
    }
}
