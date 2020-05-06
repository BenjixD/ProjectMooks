using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandSelector : MonoBehaviour
{
    public int choice = 0;
    public int maxChoice = 1;
    public bool inverse = false;

    private UnityAction callback;

    public void Initialize(int startChoice, int maxChoice, UnityAction callback = null, bool inverse = false) {
        this.choice = startChoice;
        this.maxChoice = maxChoice;
        this.callback = callback;
        this.inverse = inverse;
    }


    public int GetChoice() {
        return choice;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (!inverse) {
                this.decrementChoice();
            } else {
                this.incrementChoice();
            }

            if (this.callback != null) {
                this.callback();
            }
       }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (!inverse) {
                this.incrementChoice();
            } else {
                this.decrementChoice();
            }

            if (this.callback != null) {
                this.callback();
            }
        }
    }

    void decrementChoice() {
        if (choice == 0) {
            choice = maxChoice;
        } else {
            choice--;
        }
    }

    void incrementChoice() {
        if (choice == maxChoice) {
            choice = 0;
        } else {
            choice++;
        }
    }

}
