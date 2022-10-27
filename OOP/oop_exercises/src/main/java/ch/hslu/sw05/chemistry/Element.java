package ch.hslu.sw05.chemistry;

/**
 * Class for creating custom elements.
 */
public class Element {
    private String elementName;
    private float meltingPoint = 0;
    private float boilingPoint = 0;
    private float atomicNumber = 0;

    /**
     * Element with aggregate thresholds
     * @param meltingPoint Threshold where element transfers from solid to liquid.
     * @param boilingPoint Threshold where element transfers from liquid to gaseous.
     * @param atomicNumber The element number in the periodic table of elements.
     * @param elementName The name of the element.
     */
    Element(float meltingPoint, float boilingPoint, float atomicNumber, String elementName){
        if (meltingPoint < boilingPoint){
            this.meltingPoint = meltingPoint;
            this.boilingPoint = boilingPoint;
        }
        if (atomicNumber > 0 && atomicNumber <= 118){
            this.atomicNumber = atomicNumber;
        }
        this.elementName = elementName;
    }

    public float getAtomicNumber() { return this.atomicNumber; }

    public String getElementName() {
        return this.elementName;
    }

    public float getMeltingPoint() {
        return this.meltingPoint;
    }

    public float getBoilingPoint() {
        return this.boilingPoint;
    }

}
