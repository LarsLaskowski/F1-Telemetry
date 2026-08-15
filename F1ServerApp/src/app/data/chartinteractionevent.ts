import { ChartEvent } from "chart.js";

/**
 * Payload of a chart.js click/hover event as emitted by ng2-charts' BaseChartDirective
 * (chartClick / chartHover outputs).
 */
export interface ChartInteractionEvent
{
  event?: ChartEvent;
  active?: object[];
}
