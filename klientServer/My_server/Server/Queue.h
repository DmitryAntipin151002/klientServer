#pragma once
#include <iostream>
using namespace std;
class Queue {
private:
    int* arr;
    int front;
    int rear;
    int capacity;
    int size;
public:
    Queue(int cap = 10) {
        arr = new int[cap];
        front = 0;
        rear = -1;
        capacity = cap;
        size = 0;
    }
    Queue(const Queue& q);
    ~Queue();
    Queue& operator=(const Queue& q);
    bool operator==(const Queue& q) const;
    bool operator!=(const Queue& q) const;
    friend Queue operator+(const Queue& q, int val);
    friend Queue operator*(const Queue& q, int val);
    friend ostream& operator<<(ostream& os, const Queue& q);
    void enqueue(int val);
    void dequeue();
    void operator+=(int val);
    void print();
};
