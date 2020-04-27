"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.WordCheckComponent = void 0;
var core_1 = require("@angular/core");
var WordCheckComponent = /** @class */ (function () {
    function WordCheckComponent(verbsService, tenseService, snackBar, changeDetectorRef) {
        var _this = this;
        this.verbsService = verbsService;
        this.tenseService = tenseService;
        this.snackBar = snackBar;
        this.changeDetectorRef = changeDetectorRef;
        this.showRightAnswer = false;
        this.refreshVerbsList();
        tenseService.getTenses().subscribe(function (s) {
            _this.tenses = s;
            //let t = s.flatMap(mood => mood.exact_tenses
            //  .map(m => {
            //    return {
            //      name: mood.name + ' ' + m.name, key = m.exact_name}
            //  }));
            var flatTensesList = s.map(function (mood) { return mood.exact_tenses
                .map(function (m) {
                return {
                    name: mood.name + ' ' + m.name, key: m.exact_name
                };
            }); })
                .reduce(function (a, b) { return a.concat(b); });
            flatTensesList.forEach(function (f) {
                _this.tenses[f.key] = f.name;
            });
        });
    }
    Object.defineProperty(WordCheckComponent.prototype, "current", {
        get: function () {
            if (!this.verbs)
                return undefined;
            return this.verbs[this.currentIndex];
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(WordCheckComponent.prototype, "currentTenseName", {
        get: function () {
            return this.tenses[this.current.tense_key];
        },
        enumerable: false,
        configurable: true
    });
    WordCheckComponent.prototype.refreshVerbsList = function () {
        var _this = this;
        var verbsObs = this.verbsService.getVerbs();
        verbsObs.subscribe(function (s) {
            _this.verbs = s;
            _this.currentIndex = 0;
        });
    };
    WordCheckComponent.prototype.ngOnInit = function () {
    };
    WordCheckComponent.prototype.checkWord = function () {
        var _this = this;
        var v = (this.variant || "").trim().toLowerCase();
        var c = this.current.word.trim().toLowerCase();
        var correctAnswers = c.split(",");
        var correct = correctAnswers.indexOf(v) > -1;
        this.variant = ""; // Clear the input
        if (correct) {
            this.current["correct"] = true;
            this.snackBar.open("That's correct!", "Close", {
                duration: 1000,
            });
            this.nextWord();
        }
        else {
            this.current["correct"] = false;
            this.showRightAnswer = true;
            var barRef = this.snackBar.open("No it's wrong! Correct answer: '" + c + "'.", "Close", {
                duration: 3000,
            });
            barRef.afterDismissed().subscribe(function (s) {
                _this.showRightAnswer = false;
                _this.nextWord();
                _this.changeDetectorRef.detectChanges();
            });
        }
    };
    // Show the next verb
    WordCheckComponent.prototype.nextWord = function () {
        if (this.currentIndex < this.verbs.length - 1) {
            this.currentIndex++;
        }
        else {
            this.refreshVerbsList();
        }
    };
    WordCheckComponent = __decorate([
        core_1.Component({
            selector: 'app-word-check',
            templateUrl: './word-check.component.html',
            styleUrls: ['./word-check.component.css']
        })
    ], WordCheckComponent);
    return WordCheckComponent;
}());
exports.WordCheckComponent = WordCheckComponent;
//# sourceMappingURL=word-check.component.js.map