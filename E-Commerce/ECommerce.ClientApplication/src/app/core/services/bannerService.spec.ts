import { TestBed } from '@angular/core/testing';

import { BannerService } from './bannerService';

describe('Banner', () => {
  let service: BannerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BannerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
