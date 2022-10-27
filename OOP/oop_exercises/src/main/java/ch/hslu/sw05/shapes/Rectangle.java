package ch.hslu.sw05.shapes;

/**
 * Class for creating rectangles of different sizes and positions.
 */
public class Rectangle extends Shape{
    private int width;
    private int height;

    public Rectangle(int x, int y, int width, int height) {
        super(x, y);
        this.width = width;
        this.height = height;
    }

    public void changeDimension(int newWidth, int newHeight){
        this.width = newWidth;
        this.height = newHeight;
    }

    public int getWidth() {
        return width;
    }

    public int getHeight() {
        return height;
    }

    public void setWidth(int width) {
        this.width = width;
    }

    public void setHeight(int height) {
        this.height = height;
    }

    @Override
    public int getPerimeter() {
        return 2 * this.width + 2 * this.height;
    }

    @Override
    public int getArea() {
        return this.width * this.height;
    }
}
