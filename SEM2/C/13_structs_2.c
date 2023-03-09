#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>


struct Item{
    char *itemName;
    int quantity;
    float price;
    float amount;
};

void readItem(struct Item *item_t);
void print(const struct Item const *item_t);


int main(int argc, char const *argv[])
{
    struct Item i = {"0"};
    readItem(&i);
    print(&i);
    free(i.itemName);

    return 0;
}

void readItem(struct Item *item_t){
    item_t->itemName = (char *) malloc(sizeof(char) * 50); // allocating memory for itemName
    printf("Item name, Quantity, Price: ");
    scanf("%s %d %f", item_t->itemName, &item_t->quantity, &item_t->price);
    item_t->amount = item_t->quantity * item_t->price;
}

void print(const struct Item const *item_t){
    printf("%s\n%d\n%.2f\n%.2f\n", item_t->itemName, item_t->quantity, item_t->price, item_t->amount);
}