package ch.hslu.sw05.chemistry;

public class Element {
    private String elementName = "";
    private int meltingPoint;
    private int boilingPoint;

    private int pteNumber;

    public String getElementName() {
        return elementName;
    }

    public int getMeltingPoint() {
        return meltingPoint;
    }

    public int getBoilingPoint() {
        return boilingPoint;
    }

    /**
     * Element with aggregate thresholds
     * @param meltingPoint threshold where element transfers from solid to liquid
     * @param boilingPoint threshold where element transfers from liquid to gaseous
     */
    Element(int meltingPoint, int boilingPoint, int pteNumber, String elementName){
        this.meltingPoint = meltingPoint;
        this.boilingPoint = boilingPoint;
        this.pteNumber = pteNumber;
        this.elementName = elementName;
    }
}
