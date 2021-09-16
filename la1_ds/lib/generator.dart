import 'dart:io';
import 'package:math_expressions/math_expressions.dart';

class Generator {
  Future<Data> parceAndGenerate(String a, String c, String m, String amount, String x0, bool saveToFile) async {
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

      Data result = await generate3(_a, _c, _m, _amount, _x0, saveToFile);
      return result;
    } catch (e) {
      throw (e);
    }
    //return 0;
  }

  Future<Data> generate3(BigInt a, BigInt c, BigInt m, BigInt amount, BigInt x0, bool saveTofile) async {
    BigInt xn = x0;
    BigInt xn1 = BigInt.from(0);
    int period = 1;
    bool isFound = false;
    List<BigInt> generatedData = new List.empty(growable: true);
    generatedData.add(x0);
    //List<BigInt> generatedOutput = new List.empty(growable: true);
    //generatedOutput.add(x0);

    //for (BigInt i = BigInt.from(0); i < amount; i += BigInt.from(1)) {
    if (saveTofile) {
      var file = File("file.txt");
      if (await file.exists()) {
        await file.writeAsString("");
      }
      var stream = file.openWrite(mode: FileMode.append);
      while (!isFound && period < amount.toInt()) {
        xn1 = (a * xn + c) % m;
        if (xn1 == x0 || xn1 == xn) {
          isFound = true;
        } else {
          period++;
        }
        // if (generatedOutput.length < 10000) {
        //   generatedOutput.add(xn1);
        // }
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
      //generatedData.add(xn1);
      stream.write(generatedData);
      stream.close();
    } else {
      while (!isFound) {
        xn1 = (a * xn + c) % m;
        if (xn1 == x0 || xn1 == xn) {
          isFound = true;
        } else {
          period++;
        }
        xn = xn1;
      }
    }
    var generatedOutput = generateOnlyForUiOutput(a, c, m, x0, 10000);
    return new Data(generatedOutput, period);
  }

  List<BigInt> generateOnlyForUiOutput(BigInt a, BigInt c, BigInt m, BigInt x0, int count) {
    BigInt xn = x0;
    BigInt xn1 = BigInt.from(0);
    bool isFound = false;
    List<BigInt> output = new List<BigInt>.empty(growable: true);
    output.add(x0);
    while (!isFound && count > 0) {
      xn1 = (a * xn + c) % m;
      if (xn1 == x0 || xn1 == xn) {
        isFound = true;
      }
      output.add(xn1);
      xn = xn1;
      count--;
    }
    return output;
  }

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
