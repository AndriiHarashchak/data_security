import 'package:flutter/material.dart';

import 'generator.dart';

class Ui extends StatefulWidget {
  const Ui({Key? key}) : super(key: key);

  @override
  _UiState createState() => _UiState();
}

class _UiState extends State<Ui> {
  final aController = TextEditingController(text: "0");
  final cController = TextEditingController(text: "0");
  final resultController = TextEditingController(text: "Result");
  final mController = TextEditingController(text: "2000");
  final x0Controller = TextEditingController(text: "1");
  final amountController = TextEditingController(text: "100000");
  final _scrollController = ScrollController();
  bool isChecked = false;
  String period = '';
  Generator generator = new Generator();
  @override
  void initState() {
    super.initState();
    setState(() {
      //resultController.text = ;
    });
    //generator = new Generator();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.start,
        //crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Expanded(
          //   child: Text("Enter params"),
          //   flex: 1,
          // ),
          Expanded(
            flex: 3,
            child: Padding(
              padding: EdgeInsets.symmetric(vertical: 5, horizontal: 40),
              child: _getOperatingFileds(),
            ),
          ),
          Expanded(
            flex: 8,
            child: Padding(
              padding: EdgeInsets.symmetric(vertical: 20, horizontal: 40),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    "Results",
                    style: TextStyle(fontWeight: FontWeight.bold, fontSize: 24),
                  ),
                  Text(
                    "Period: " + period,
                    style: TextStyle(fontWeight: FontWeight.w500, fontSize: 20),
                  ),
                  Text(
                    "Generated numbers (only period +1  or first 10000 if period and amount > 10000): ",
                    style: TextStyle(fontWeight: FontWeight.w500, fontSize: 20),
                  ),
                  Container(
                    width: double.infinity,
                    height: 300,
                    decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.all(Radius.circular(5)),
                        border: Border.all(width: 1, color: Colors.black)),
                    child: Scrollbar(
                        isAlwaysShown: true,
                        controller: _scrollController,
                        child: SingleChildScrollView(
                          controller: _scrollController,
                          child: Padding(
                            padding: EdgeInsets.all(10),
                            child: Text(
                              resultController.text,
                              //style: TextStyle(backgroundColor: Colors.red),
                            ),
                          ),
                        )),
                  ),
                ],
              ),
            ),
          )
        ],
      ),
    );
  }

  Widget _getOperatingFileds() {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Expanded(
          flex: 1,
          child: Padding(
            padding: EdgeInsets.symmetric(horizontal: 5, vertical: 10),
            child: TextField(
              controller: aController,
              keyboardType: TextInputType.number,
              // inputFormatters: <TextInputFormatter>[
              //   FilteringTextInputFormatter.allow(RegExp(r'[0-9]')),
              // ],
              decoration:
                  InputDecoration(border: OutlineInputBorder(), labelText: "a"),
            ),
          ),
        ),
        Expanded(
          flex: 1,
          child: Padding(
            padding: EdgeInsets.symmetric(horizontal: 5, vertical: 10),
            child: TextField(
              controller: cController,
              keyboardType: TextInputType.number,
              // inputFormatters: <TextInputFormatter>[
              //   FilteringTextInputFormatter.allow(RegExp(r'[0-9]')),
              // ],
              decoration:
                  InputDecoration(border: OutlineInputBorder(), labelText: "c"),
            ),
          ),
        ),
        Expanded(
          flex: 1,
          child: Padding(
            padding: EdgeInsets.symmetric(horizontal: 5, vertical: 10),
            child: TextField(
              controller: mController,
              keyboardType: TextInputType.number,
              // inputFormatters: <TextInputFormatter>[
              //   FilteringTextInputFormatter.allow(RegExp(r'[0-9]')),
              // ],
              decoration:
                  InputDecoration(border: OutlineInputBorder(), labelText: "m"),
            ),
          ),
        ),
        Expanded(
          flex: 1,
          child: Padding(
            padding: EdgeInsets.symmetric(horizontal: 5, vertical: 10),
            child: TextField(
              controller: amountController,
              keyboardType: TextInputType.number,
              // inputFormatters: <TextInputFormatter>[
              //   FilteringTextInputFormatter.allow(RegExp(r'[0-9]')),
              // ],
              decoration: InputDecoration(
                  border: OutlineInputBorder(), labelText: "amount"),
            ),
          ),
        ),
        Expanded(
          flex: 1,
          child: Padding(
            padding: EdgeInsets.symmetric(horizontal: 5, vertical: 10),
            child: TextField(
              controller: x0Controller,
              keyboardType: TextInputType.number,
              // inputFormatters: <TextInputFormatter>[
              //   FilteringTextInputFormatter.allow(RegExp(r'[0-9]')),
              // ],
              decoration: InputDecoration(
                  border: OutlineInputBorder(), labelText: "X0"),
            ),
          ),
        ),
        Expanded(
            child: Column(
          children: [
            Text("Save data to file?"),
            Checkbox(
              value: isChecked,
              onChanged: (newValue) {
                setState(() {
                  isChecked = !isChecked;
                });
              },
            ),
          ],
        )),
        Expanded(
          flex: 1,
          child: Padding(
            padding: EdgeInsets.symmetric(vertical: 10),
            child: ElevatedButton(
                onPressed: () {
                  generate();
                },
                child: Text("Randomize")),
          ),
        ),
      ],
    );
  }

  void generate() async {
    try {
      Data a = await generator.parceAndGenerate(
          aController.text,
          cController.text,
          mController.text,
          amountController.text,
          x0Controller.text,
          isChecked);
      setState(() {
        resultController.text = a.periodArray.toString();
        period = a.period.toString();
      });
    } catch (e) {
      setState(() {
        resultController.text = e.toString();
        period = 0.toString();
      });
    }
    //, c, m, amount, x0
    // try {
    //   var m = int.parse(mController.text);
    //   if (m < 0) {
    //     setState(() {
    //       resultController.text = "error: m can`t be less than 1";
    //     });
    //     return;
    //   }
    //   var a = int.parse(aController.text);
    //   if (a <= 0 && a > m) {
    //     setState(() {
    //       resultController.text =
    //           "error: a can`t be less than 0 and a can`t be greater than m";
    //     });
    //     return;
    //   }
    //   var c = int.parse(cController.text);
    //   if (c <= 0 && c > m) {
    //     setState(() {
    //       resultController.text =
    //           "error: c can`t be less than 0 and a can`t be greater than m";
    //     });
    //     return;
    //   }
    //   var x0 = int.parse(x0Controller.text);
    //   if (x0 <= 0 && x0 > m) {
    //     setState(() {
    //       resultController.text =
    //           "error: X0 can`t be less than 0 and a can`t be greater than m";
    //     });
    //     return;
    //   }
    //   var amount = int.parse(amountController.text);
    //   if (amount <= 0) {
    //     setState(() {
    //       resultController.text = "amount can`t be less than 1";
    //     });
    //   }
    //   var result = generator.generate(a, c, m, amount, x0);
    //   setState(() {
    //     resultController.text = result.toString();
    //     //print("Done");
    //   });
    // } catch (e) {
    //   setState(() {
    //     resultController.text = e.toString();
    //   });
    // }
  }
}
