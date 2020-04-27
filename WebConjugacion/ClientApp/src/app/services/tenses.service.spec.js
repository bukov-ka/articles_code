"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var tenses_service_1 = require("./tenses.service");
describe('TensesService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(tenses_service_1.TensesService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=tenses.service.spec.js.map