import 'dart:io';
import 'package:math_expressions/math_expressions.dart';

class Generator {
  Future<Data> parceAndGenerate(
      String a, String c, String m, String amount, String x0) async {
    //
    try {
      Parser p = new Parser();
      ContextModel cm = ContextModel();
      Expression exp = p.parse(m);
      BigInt _m = BigInt.from(exp.evaluate(EvaluationType.REAL, cm));
      if (_m <= BigInt.from(0)) {
        throw new Exception("m must be greater that 0");
      }
      exp = p.parse(a);
      BigInt _a = BigInt.from(exp.evaluate(EvaluationType.REAL, cm));
      if (_a < BigInt.from(0) || _a >= _m) {
        throw new Exception("a must be greater or equal 0 and less than m");
      }
      exp = p.parse(c);
      BigInt _c = BigInt.from(exp.evaluate(EvaluationType.REAL, cm));
      if (_c < BigInt.from(0) || _c >= _m) {
        throw new Exception("c must be greater or equal 0 and less than m");
      }
      exp = p.parse(amount);
      BigInt _amount = BigInt.from(exp.evaluate(EvaluationType.REAL, cm));
      if (_amount <= BigInt.from(0)) {
        throw new Exception("amount must be greater than 0");
      }
      exp = p.parse(x0);
      BigInt _x0 = BigInt.from(exp.evaluate(EvaluationType.REAL, cm));
      if (_x0 < BigInt.from(0) || _x0 >= _m) {
        throw new Exception("x0 must be greater or equal 0 and less than m");
      }

      Data result = await generate3(_a, _c, _m, _amount, _x0);
      return result;
    } catch (e) {
      throw (e);
    }
    //return 0;
  }

  Future<Data> generate3(
      BigInt a, BigInt c, BigInt m, BigInt amount, BigInt x0) async {
    BigInt xn = x0;
    BigInt xn1 = BigInt.from(0);
    int period = 1;
    bool isFound = false;
    List<BigInt> generatedData = new List.empty(growable: true);
    generatedData.add(x0);
    List<BigInt> generatedOutput = new List.empty(growable: true);
    generatedOutput.add(x0);
    var file = File("file.txt");
    if (await file.exists()) {
      await file.writeAsString("");
    }
    var stream = file.openWrite(mode: FileMode.append);
    //for (BigInt i = BigInt.from(0); i < amount; i += BigInt.from(1)) {
    while (!isFound) {
      xn1 = (a * xn + c) % m;
      if (isFound == false) {
        if (xn1 == x0 || xn1 == xn) {
          isFound = true;
        } else {
          period++;
        }
      }
      if (!isFound && generatedOutput.length < 10000) {
        generatedOutput.add(xn1);
      }
      //if (period < 100000000)
      if (generatedData.length < 100000) {
        generatedData.add(xn1);
      } else {
        // await file.writeAsString(generatedData.toString(),
        //     mode: FileMode.append);
        stream.write(generatedData);
        await stream.flush();
        generatedData.clear();
        generatedData.add(xn1);
      }
      xn = xn1;
    }
    //xn1 = (a * xn + c) % m;
    stream.write(xn1);
    // if (generatedOutput.length < 10000) {
    //   generatedOutput.add(x0);
    // }
    if (period > 10000) {
      generatedOutput.add(xn1);
    } else {
      generatedOutput.add(x0);
    }
    print(period);
    stream.close();
    return new Data(generatedOutput, period);
  }

  // Data generate2(int a, int c, int m, int amount, int x0) {
  //   int xn = x0;
  //   int xn1 = 0;
  //   int period = 1;

  //   bool isFound = false;
  //   while (isFound == false && period <= amount) {
  //     xn1 = (a * xn + c) % m;
  //     if (xn1 == x0) {
  //       isFound = true;
  //     }
  //     ++period;
  //     xn = xn1;
  //   }
  //   print(period);
  //   return new Data([], period);
  // }

  // Data generate(int a, int c, int m, int amount, int x0) {
  //   int xn = x0;
  //   int xn1 = 0;
  //   var file = File("file.txt");
  //   var array = new List<int>.empty(growable: true);
  //   var periodArray = new List<int>.empty(growable: true);
  //   periodArray.add(x0);
  //   int period = 1;
  //   bool isPeriodCounted = false;
  //   array.add(x0);
  //   bool append = false;
  //   for (int i = 0; i <= amount; ++i) {
  //     xn1 = (a * xn + c) % m;
  //     //save
  //     //array[i] = xn1;
  //     try {
  //       if (array.length >= 10000) {
  //         saveData(file, array, append);
  //         append = true;
  //         array.clear();
  //       }
  //       array.add(xn1);

  //       if (x0 == xn1 && !isPeriodCounted) {
  //         isPeriodCounted = true;
  //       } else if (isPeriodCounted) {
  //       } else {
  //         period++;
  //         periodArray.add(xn1);
  //       }

  //       // if (!isPeriodCounted) {
  //       //   period++;
  //       //   periodArray.add(xn1);
  //       // } else if (xn1 == x0) {
  //       //   isPeriodCounted = true;
  //       // }
  //       xn = xn1;
  //     } catch (e) {
  //       print(e.toString());
  //       throw (e);
  //     }

  //     //Console.WriteLine(xn1 + " ");
  //   }
  //   saveData(file, array, append);
  //   Data result = new Data(BigInt,periodArray, period);
  //   return result;
  // }

  Future<bool> saveData(File file, List<int> array1, bool append) async {
    try {
      if (append == true) {
        await file.writeAsString(array1.toString(), mode: FileMode.append);
      } else {
        await file.writeAsString(array1.toString());
      }
    } catch (e) {
      throw (e.toString());
    }

    return true;
  }
}

class Data {
  List<BigInt> periodArray = [];
  int period = 0;
  Data(List<BigInt> periodArrayIn, int peroidIn) {
    this.periodArray = periodArrayIn;
    this.period = peroidIn;
  }
}
