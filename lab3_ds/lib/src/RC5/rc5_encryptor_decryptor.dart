import 'dart:math';
import 'dart:typed_data';

enum EncryptionType { Encrypt, Decrypt }

class RC5 {
  int w = 32; // word size;

  int _r = 0;
  Uint8List _k = Uint8List(0);
  Uint32List _s = Uint32List(0);

  int Pw = 0xB7E15162;
  int Qw = 0x9E3779B9;
  RC5(Uint8List key, int rounds) {
    if (rounds > 0) {
      _r = rounds;
    } else {
      throw ArgumentError("rounds must be more or equal than 0");
    }
    _k = key;
    _generateExtendedKeys();
  } //

  void _generateExtendedKeys() {
    int u = w ~/ 8;
    int b = _k.length;
    int c = b % u > 0 ? b ~/ u + 1 : b ~/ u;
    List<int> L = List.filled(c, 0);

    for (int i = b - 1; i >= 0; i--) {
      L[i ~/ u] = ROL(L[i ~/ u], 8) + _k[i];
    }

    int size = (2 * _r + 2).toInt();
    _s = Uint32List(size);
    _s[0] = Pw;
    for (int i = 1; i < size; i++) {
      _s[i] = _s[i - 1] + Qw;
    }

    int A = 0, B = 0;
    int i = 0, j = 0;
    int n = (3 * max(size, c)).toInt();
    for (int k = 0; k < n; k++) {
      _s[i] = ROL(_s[i] + A + B, 3);
      A = _s[i];
      var sumex = A + B + L[j];
      int sum = A + B;
      L[j] = ROL(sumex, sum);
      B = L[j];
      i = (i + 1) % size;
      j = (j + 1) % c;
    }
  }

  static int BITS_IN_INTEGER = 32;
  static int INTEGER_MASK = (1 << BITS_IN_INTEGER) - 1;

  static int ROL(int x, int n) {
    return INTEGER_MASK & ((x << (n & 0x1F).toInt()) | (x >> ((BITS_IN_INTEGER - n) & 0x1F)));
    // if (BITS_IN_INTEGER - n >= 0) {
    //   return INTEGER_MASK & ((x << (n & 0x1F).toInt()) | (x >> ((BITS_IN_INTEGER - n) & 0x1F)));
    // } else {
    //   return INTEGER_MASK & ((x << (n & 0x1F).toInt()) | ((x ~/ pow(2, (BITS_IN_INTEGER - n))).toInt()));
    // }
  }

  // int ROL(int word, int offset) {
  //   int r1, r2;
  //   r1 = word << (offset & 0x3F);
  //   r1 &= INTEGER_MASK;
  //   r2 = word >> ((w - offset) & 0x3F); //~/ 2
  //   r2 &= INTEGER_MASK;
  //   return (r1 | r2);
  // }

  int ROR(int word, int offset) {
    int r1, r2;
    r1 = word >> (offset & 0x1F);
    r2 = word << ((w - offset) & 0x1F); //~/ 2
    return (r1 | r2);
  }

  Uint8List encrypt_decrypt(Uint8List inputData, EncryptionType operation) {
    List<int> endecryptedData = List.empty(growable: true);

    int lenght = inputData.length;
    //int processedBytes = 0;
    for (int k = 0; k < lenght; k += 4) {
      int end = k + 4;
      var block = Uint8List(4);
      if (end >= lenght) {
        end = lenght - 1;
        block.setAll(0, inputData.sublist(k));
      } else {
        block = Uint8List.sublistView(inputData, k, end);
      }
      if (operation == EncryptionType.Encrypt) {
        var endecryptedBlock = _encryptBlock(block);
        endecryptedData.addAll(endecryptedBlock);
      } else {
        var endecryptedBlock = _decryptBlock(block);
        endecryptedData.addAll(endecryptedBlock);
      }
      //processedBytes += end - k;
    }
    // if (processedBytes < lenght) {
    //   var block = Uint8List(4);
    //   block.setAll(0, inputData.sublist(processedBytes));
    //   encryptedData.addAll(_encryptBlock(block).sublist(0, lenght - processedBytes));
    // }
    return Uint8List.fromList(endecryptedData);
  }

  Uint8List _encryptBlock(Uint8List inputData) {
    int a = get16bitValue(inputData, 0);
    int b = get16bitValue(inputData, 2);

    a = a + _s[0];
    b = b + _s[1];
    for (int i = 1; i < _r + 1; i++) {
      a = (ROL(a ^ b, b) + _s[2 * i]).toInt();
      b = (ROL(b ^ a, a) + _s[2 * i + 1]).toInt();
    }
    Uint8List outputData = Uint8List(inputData.length)
      ..buffer.asByteData().setInt16(0, a, Endian.little)
      ..buffer.asByteData().setInt16(2, b, Endian.little);
    return outputData;
  }

  Uint8List _decryptBlock(Uint8List inputData) {
    int a = get16bitValue(inputData, 0);
    int b = get16bitValue(inputData, 2);

    //a = a + _s[0];
    //b = b + _s[1];
    for (int i = _r; i > 0; i--) {
      b = ((ROR(b - _s[2 * i + 1], a)) ^ a).toInt();
      a = ((ROR(a - _s[2 * i], b)) ^ b).toInt();
    }
    a = a - _s[0];
    b = b - _s[1];
    Uint8List outputData = Uint8List(inputData.length)
      ..buffer.asByteData().setInt16(0, a, Endian.little)
      ..buffer.asByteData().setInt16(2, b, Endian.little);
    return outputData;
  }

  static int get16bitValue(Uint8List data, int offset) => ByteData.view(data.buffer).getUint16(offset, Endian.little);
  static Uint8List int16LitteEndianBytes(Uint8List list, int value, int offset) => list..buffer.asByteData().setInt16(0, value, Endian.little);
}
