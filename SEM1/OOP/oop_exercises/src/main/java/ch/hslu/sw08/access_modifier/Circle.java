package ch.hslu.sw08.access_modifier;

/**
 * Class for creating circles of different sizes and positions.
 * Note: this class doesn't support the creating of ellipses.
 */
public class Circle extends Shape {
    private int diameter;

    public Circle(final int x, final int y, final int diameter) {
        super(x, y);
        this.diameter = diameter;
    }

    public int getDiameter() {
        return diameter;
    }

    public void setDiameter(int diameter) {
        this.diameter = diameter;
    }

    @Override
    public int getPerimeter() {
        return (int)(Math.PI * this.diameter + 0.5);
    }

    @Override
    public int getArea() {
        return (int)(Math.pow(this.diameter, 2) * Math.PI / 4 + 0.5);
    }
}
