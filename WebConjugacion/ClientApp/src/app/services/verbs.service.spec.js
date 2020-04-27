"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var verbs_service_1 = require("./verbs.service");
describe('VerbsService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(verbs_service_1.VerbsService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=verbs.service.spec.js.map