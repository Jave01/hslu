package ch.hslu.sw08.access_modifier;

/**
 * Class for creating a 2-dimensional, equally sized rectangle.
 */
public final class Square extends Shape {
    private int length;

    protected Square(int x, int y, int length) {

        super(x, y);
        this.length = length;
    }

    /**
     * Returns the perimeter of the square.
     * {@inheritDoc}
     */
    @Override
    public int getPerimeter() {
        return 4 * length;
    }

    /**
     * Return area of square
     * {@inheritDoc}
     */
    @Override
    public int getArea() {
        return length * length;
    }

    public int getLength() {
        return length;
    }

    public void setLength(int length) {
        this.length = length;
    }
}
