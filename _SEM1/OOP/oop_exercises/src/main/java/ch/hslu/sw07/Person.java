package ch.hslu.sw07;

import java.util.Objects;

public final class Person implements Comparable<Person>{
    private final int id;
    private final String lastname;
    private final String firstname;

    public Person(int id, String firstname, String lastname){
        this.id = id;
        this.firstname = firstname;
        this.lastname = lastname;
    }

    public String getLastName() {
        return lastname;
    }

    public String getFirstname() {
        return firstname;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;

        if (!(o instanceof Person person)) return false;

        return  this.id == person.id;
    }

    @Override
    public String toString(){
        return "Person[" + this.firstname + " " + this.lastname + ", " + this.id;
    }
    @Override
    public int hashCode() {
        return Objects.hash(this.id);
    }

    @Override
    public int compareTo(Person other){
        return CharSequence.compare((this.lastname + this.firstname), (other.lastname + other.firstname));
    }
}
