package ch.hslu.sw03;
public class Printer {
    
    /** 
     * @param width
     * @param height
     */
    public void drawBox(int width, int height){
        if (width < 3 || height < 3){
            System.out.println("Values too small");
            return;
        }
        for (int i = 0; i < height; i++){
            for (int j = 0; j < width; j++){
                if (i == 0 || i == height - 1 || j == 0 || j == width - 1){
                    System.out.print('#');
                }
                else{
                    System.out.print(' ');
                }
            }
            System.out.println();
        }
        System.out.println("Drawn successfully");
    }
}
