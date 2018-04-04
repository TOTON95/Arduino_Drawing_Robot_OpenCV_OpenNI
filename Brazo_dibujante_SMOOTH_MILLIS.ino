//CREATED BY TOTON (ALEXIS GUIJARRO)@2016. FOR EDUCATIONAL PURPOSES ONLY.
#include <Servo.h>

//Comm
String buff;
String anw;
String num1;
String num2;
char chr;
bool cambio = false;
int ang1 = 0;
int ang2 = 0;
int led = 13;

//Servos
Servo servoA;
Servo servoB;

int pos1;
int pos2;

//SMOOTH
int pos1_last;
int pos2_last;
int retraso = 10;
int i;
int j;

void setup() {
  Serial.begin(9600);
  servoA.attach(10);
  servoB.attach(9);
  servoA.write(90);
  servoB.write(180);
  pos1_last = 90;
  pos2_last = 180;
  pinMode(led, OUTPUT);
}

void loop() {
  if (Serial.available() > 0) {
    chr = ((byte)Serial.read());
    if (chr == '+')
    {
      delay(15);
      Serial.println("OK");
    }
    buff += chr;
    delay(5);
    if (chr == '*') {
      for (int i = 0; i < buff.length(); i++)
      {
        if (buff[i] == 'A')
        {
          anw += "A: ";
          cambio = false;
        }
        if (buff[i] == 'B')
        {
          anw += " B: ";
          cambio = true;
        }
        if (isDigit(buff[i]))
        {
          if (cambio == false)
          {
            num1 += buff[i];
            ang1 = num1.toInt();
          }
          if (cambio == true)
          {
            num2 += buff[i];
            ang2 = num2.toInt();
          }
          anw += buff[i];
        }
      }

      //Asignando valores
      pos1 = ang1;
      pos2 = ang2;
      //pos2 = 180 - ang2;

      //Escribir angulos
      //tratando de dar tiempo en C#
      //moviendo servos
      while (pos1_last != pos1 && pos2_last != pos2 || pos1_last == pos1 && pos2_last != pos2 || pos1_last != pos1 && pos2_last == pos2)
      {
        if (pos1_last < pos1)
        {
          if (pos1 - pos1_last > 30)
          {
            pos1_last += 2;
          }
          else {
            pos1_last += 1;
          }
        }
        if (pos2_last < pos2)
        {
          if (pos2 - pos2_last > 10)
          {
            pos2_last += 4;
          }
          else {
            pos2_last += 1;
          }
        }
        if (pos1_last > pos1)
        {
          if (pos1_last - pos1 > 30)
          {
            pos1_last -= 2;
          }
          else {
            pos1_last -= 1;
          }
        }
        if (pos2_last > pos2)
        {
          if (pos2_last - pos2 > 10)
          {
            pos2_last -= 4;
          }
          else {
            pos2_last -= 1;
          }
        }
        /*Serial.print(pos1_last);
        Serial.println(pos2_last);*/
        servoA.write(pos1_last);
        servoB.write(180 - pos2_last);
      }

      //Desechar datos
      anw = "";
      buff = "";
      num1 = "";
      num2 = "";
      pos1 = 0;
      pos2 = 0;
    }

  }

}
