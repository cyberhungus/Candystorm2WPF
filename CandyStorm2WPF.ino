
//needed for serial communication system
const byte numChars = 64;
char receivedChars[numChars];

boolean newData = false;
int dataNumber = 0;

//definitions for servo - connect servo control wire to 9 (or change this field)

int relayPinA = 13;
int relayPinB = 12;

//Connect Sensors to these pins, or change fields
int ButtonPin = 23;

int lastState = 0;

void setup() {
  Serial.begin(115200);
  Serial.println("<Arduino is ready>");

  //make sensor pins input pins
  pinMode(ButtonPin, INPUT_PULLUP);


  pinMode(relayPinA, OUTPUT);
  pinMode(relayPinB, OUTPUT);


}

void loop() {
  recvWithStartEndMarkers();
  showNewData();
  readSensors();

}

//checks the sensors and modifiies toSend string
void readSensors() {
  int state = 0;
  //erster Sensor
  state = digitalRead(ButtonPin);
  // Serial.println("Sensor A: " + state);
  if (state == 0 and lastState == 1) {
    Serial.println("<0001>");
  }
  lastState = state;

}


//receives messages dilineated by < and > symbols
void recvWithStartEndMarkers() {
  static boolean recvInProgress = false;
  static byte ndx = 0;
  char startMarker = '<';
  char endMarker = '>';
  char rc;

  while (Serial.available() > 0 && newData == false) {
    rc = Serial.read();

    if (recvInProgress == true) {
      if (rc != endMarker) {
        receivedChars[ndx] = rc;
        ndx++;
        if (ndx >= numChars) {
          ndx = numChars - 1;
        }
      } else {
        receivedChars[ndx] = '\0';  // terminate the string
        recvInProgress = false;
        ndx = 0;
        newData = true;
      }
    }

    else if (rc == startMarker) {
      recvInProgress = true;
    }
  }
}

//when a complete message was received, this function is called
//analyses received message and moves motor
void showNewData() {
  if (newData == true) {
    newData = false;
    String incomingByte = String(receivedChars);
    //check if system SHOULD act on this message
    if (incomingByte.substring(0, 4).equals("SYS-")) {
      //GET PARAMETERS as string
      String relayString = incomingByte.substring(4, 7);
      int relayint = relayString.toInt();
      if (relayint == 1) {
        updateRelay(false);
      }
      else {
        updateRelay(true);
      }
    }
  }
}




void updateRelay(boolean A) {
  if (A == false) {
    digitalWrite(relayPinA, HIGH);
    digitalWrite(relayPinB, HIGH);
  }
  else {
    digitalWrite(relayPinA, LOW);
    digitalWrite(relayPinB, LOW);
  }
}
