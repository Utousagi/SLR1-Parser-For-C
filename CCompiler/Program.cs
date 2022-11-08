using CCompiler;
using CCompiler.utils;

var syntax = new SyntaxAnalyzer();
var txt = File.OpenText("../../../test.txt").ReadToEnd();
syntax.Process(txt);
