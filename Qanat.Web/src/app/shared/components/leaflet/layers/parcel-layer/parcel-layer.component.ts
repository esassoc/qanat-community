import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, EventEmitter, Input, OnChanges, Output, SimpleChange } from '@angular/core';
import * as L from 'leaflet';


import { MapLayerBase } from '../map-layer-base.component';
import { WfsService } from 'src/app/shared/services/wfs.service';
@Component({
  selector: 'parcel-layer',
  standalone: true,
  imports: [CommonModule, MapLayerBase],
  templateUrl: './parcel-layer.component.html',
  styleUrls: ['./parcel-layer.component.scss']
})
export class ParcelLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
  @Input() controlTitle: string = 'My Parcels';
  @Input({ required: true }) geographyID: number;
  @Input() parcelIDs: number[];
  @Input() selectedParcelID: number;

  @Output() layerBoundsCalculated = new EventEmitter();
  @Output() parcelSelected = new EventEmitter<number>();
  
  public isLoading: boolean = false;
  public layer;

  private defaultStyle = {
    'color': '#3388ff',
    'weight': 2,
    'opacity': 0.65,
    'fillOpacity': 0.1
  };

  private highlightStyle = {
    'color': '#fcfc12',
    'weight': 2,
    'opacity': 0.65,
    'fillOpacity': 0.1
  };

  constructor(
    private wfsService: WfsService
  ) {
    super();
  }

  ngOnChanges(changes: any): void {
    if (changes.selectedParcelID) {

      if (changes.selectedParcelID.previousValue == changes.selectedParcelID.currentValue) return;
      this.selectedParcelID == changes.selectedParcelID.currentValue;
      this.highlightSelectedParcel();

    } else if(Object.values(changes).some((x: SimpleChange) => x.firstChange === false)){
      this.updateLayer();
    }
  }

  ngAfterViewInit(): void {
    this.setupLayer();
    this.updateLayer();
  }

  updateLayer() {
    this.layer.clearLayers();

    let cql_filter = `GeographyID = ${this.geographyID}`;
    if (this.parcelIDs) {
      cql_filter += ` and ParcelID in (${this.parcelIDs.join(',')})`;
    }

    this.wfsService.getGeoserverWFSLayer(null, 'Qanat:AllParcels', cql_filter)
      .subscribe(response => {
        if (response.length == 0) return;

        response.forEach((feature: any) => {
          const geoJson = L.geoJSON(feature, {
            style: this.defaultStyle
          });

          //IMPORTANT: THIS ONLY WORKS BECAUSE I'VE INSTALLED @angular/elements AND CONFIGURED THIS IN THE app.module.ts bootstrapping
          geoJson.bindPopup(`<parcel-popup-custom-element parcel-id="${feature.properties.ParcelID}"></parcel-popup-custom-element>`, {
            maxWidth: 475,
            keepInView: true
          });

          geoJson.on('mouseover', (e) => {
            geoJson.setStyle({ 'fillOpacity': 0.5 });
          });
          geoJson.on('mouseout', (e) => {
            geoJson.setStyle({ 'fillOpacity': 0.1 });
          });

          geoJson.on('click', (e) => {
            this.onParcelSelected(Number(feature.properties.ParcelID));
          });

          this.layer.addLayer(geoJson);
        });

        this.layer.addTo(this.map);
        this.map.fitBounds(this.layer.getBounds());

        this.isLoading = false;
      });
  }

  private onParcelSelected(parcelID: number) {
    this.selectedParcelID = parcelID;
    this.highlightSelectedParcel();

    this.parcelSelected.emit(parcelID);
  }

  private highlightSelectedParcel() {
    // clear styles
    this.layer.setStyle(this.defaultStyle);
    this.map.closePopup();

    // loop through the allWaterAccountsFeatureGroup
    this.layer.eachLayer((layer) => {
      const geoJsonLayers = layer.getLayers();
      if (geoJsonLayers[0].feature.properties.ParcelID == this.selectedParcelID) {
        layer.setStyle(this.highlightStyle);
        layer.openPopup();
        this.map.fitBounds(layer.getBounds());
      }
    });
  }

  private setupLayer() {
    this.layer = L.geoJSON();
    this.initLayer();
  }
}
