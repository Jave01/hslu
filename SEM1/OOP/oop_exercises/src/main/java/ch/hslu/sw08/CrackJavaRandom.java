package ch.hslu.sw08;

import java.util.Random;

public class CrackJavaRandom{
    public static final void main(String[] args){
        ReplicatedRandom rr = new ReplicatedRandom();
        Random r = new Random();

//        rr.replicateState(r.nextInt(), r.nextInt());
//        System.out.println(r.nextInt() == rr.nextInt()); // True
//        System.out.println(r.nextInt() == rr.nextInt()); // True
//        System.out.println(r.nextInt() == rr.nextInt()); // True

        rr.replicateState(1767252209 , -1664600482); // Exercise 8 from ISF sw08 -> 2082145366
        System.out.println(rr.nextInt());
    }
}