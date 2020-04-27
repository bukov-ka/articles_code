import { TestBed } from '@angular/core/testing';

import { VerbsService } from './verbs.service';

describe('VerbsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: VerbsService = TestBed.get(VerbsService);
    expect(service).toBeTruthy();
  });
});
