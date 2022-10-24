package ch.hslu.sw05.chemistry;

public class Element {
    private String elementName = "";
    private int meltingPoint;
    private int boilingPoint;
    private int pteNumber;

    public int getPteNumber() { return this.pteNumber; }

    public String getElementName() {
        return this.elementName;
    }

    public int getMeltingPoint() {
        return this.meltingPoint;
    }

    public int getBoilingPoint() {
        return this.boilingPoint;
    }

    /**
     * Element with aggregate thresholds
     * @param meltingPoint Threshold where element transfers from solid to liquid.
     * @param boilingPoint Threshold where element transfers from liquid to gaseous.
     * @param pteNumber The element number in the periodic table of elements.
     * @param elementName The name of the element.
     */
    Element(int meltingPoint, int boilingPoint, int pteNumber, String elementName){
        if (meltingPoint < boilingPoint){
            this.meltingPoint = meltingPoint;
            this.boilingPoint = boilingPoint;
        }
        if (pteNumber > 0 && pteNumber < 125){
            this.pteNumber = pteNumber;
        }
        if (elementName != ""){
            this.elementName = elementName;
        }
    }
}
