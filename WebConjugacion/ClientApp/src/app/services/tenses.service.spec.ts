import { TestBed } from '@angular/core/testing';

import { TensesService } from './tenses.service';

describe('TensesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TensesService = TestBed.get(TensesService);
    expect(service).toBeTruthy();
  });
});
