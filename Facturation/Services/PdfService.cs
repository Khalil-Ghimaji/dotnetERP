using Persistence.entities.Facturation;

namespace Facturation.Services;

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

public class PDFService: IPDFService 
{
    public byte[] GenererFacturePDF(Facture facture)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));
                page.Content().Column(column =>
                {
                    column.Item().Text($"Facture #{facture.FactureId}")
                        .FontSize(20)
                        .Bold()
                        .FontColor(Colors.Blue.Darken3);

                    column.Item().Text($"Date de génération : {facture.DateGeneration:dd/MM/yyyy}");
                    column.Item().Text($"Commande ID : {facture.CommandeId}");
                    column.Item().Text($"Montant Total : {facture.MontantTotal:C}");
                    column.Item().Text($"Statut de la facture : {facture.StatusFacture}");

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        table.Cell().Element(CellStyle).Text("Description");
                        table.Cell().Element(CellStyle).Text("Montant");

                        table.Cell().Element(CellStyle).Text("Produit A");
                        table.Cell().Element(CellStyle).Text("100 €");

                        table.Cell().Element(CellStyle).Text("Produit B");
                        table.Cell().Element(CellStyle).Text("50 €");

                        table.Cell().Element(CellStyle).Text("Total");
                        table.Cell().Element(CellStyle).Text(facture.MontantTotal.ToString("C"));
                    });
                });
            });
        }).GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.Border(1).Padding(5).AlignCenter().AlignMiddle();
    }
}
