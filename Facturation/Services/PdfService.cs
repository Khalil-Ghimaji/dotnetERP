using Persistence.entities.Facturation;
using Persistence.entities.Commande;

namespace Facturation.Services;

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using System.Globalization;

public class PDFService : IPDFService
{
    public byte[] GenererFacturePDF(Facture facture)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                // En-tête de la facture
                page.Header().Column(column =>
                {
                    column.Item().AlignCenter().Text("Facture")
                        .FontSize(24)
                        .Bold()
                        .FontColor(Colors.Blue.Darken3);

                    column.Item().AlignCenter().Text($"Numéro de facture : #{facture.FactureId}")
                        .FontSize(16)
                        .SemiBold()
                        .FontColor(Colors.Grey.Darken2);

                    column.Item().AlignCenter().Text($"Date de génération : {facture.DateGeneration:dd/MM/yyyy}")
                        .FontSize(14)
                        .FontColor(Colors.Grey.Darken1);
                });

                // Informations de la commande et du client
                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    // Informations de la commande
                    column.Item().Text("Informations de la commande")
                        .FontSize(18)
                        .Bold()
                        .FontColor(Colors.Blue.Darken3);

                    column.Item().Text($"Commande ID : {facture.CommandeId}")
                        .FontSize(14);

                    if (facture.Commande != null)
                    {
                        column.Item().Text($"Date de la commande : {facture.Commande.dateCommande:dd/MM/yyyy}")
                            .FontSize(14);

                        column.Item().Text($"Statut de la commande : {facture.Commande.status}")
                            .FontSize(14);

                        // Informations du client
                        column.Item().Text("Informations du client")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().Text($"Nom du client : {facture.Commande.client.nom}")
                            .FontSize(14);

                        column.Item().Text($"Adresse : {facture.Commande.client.address}")
                            .FontSize(14);

                        column.Item().Text($"Téléphone : {facture.Commande.client.telephone}")
                            .FontSize(14);

                        // Tableau des articles de la commande
                        column.Item().Text("Détails des articles")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Produit
                                columns.RelativeColumn(1); // Quantité
                                columns.RelativeColumn(1); // Prix unitaire
                                columns.RelativeColumn(1); // Total
                            });

                            // En-tête du tableau
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle).Text("Produit");
                                header.Cell().Element(HeaderCellStyle).Text("Quantité");
                                header.Cell().Element(HeaderCellStyle).Text("Prix unitaire");
                                header.Cell().Element(HeaderCellStyle).Text("Total");

                                static IContainer HeaderCellStyle(IContainer container)
                                {
                                    return container
                                        .Background(Colors.Blue.Darken3)
                                        .Padding(5)
                                        .AlignCenter()
                                        .AlignMiddle()
;
                                }
                            });

                            // Corps du tableau
                            foreach (var article in facture.Commande.articles)
                            {
                                table.Cell().Element(CellStyle).Text(article.produit.Nom);
                                table.Cell().Element(CellStyle).Text(article.quantite.ToString());
                                table.Cell().Element(CellStyle).Text(article.prix.ToString("C", new CultureInfo("en-TN") { NumberFormat = { CurrencySymbol = " TND", CurrencyPositivePattern = 3 } }));
                                table.Cell().Element(CellStyle).Text((article.quantite * article.prix).ToString("C", new CultureInfo("en-TN"){ NumberFormat = { CurrencySymbol = " TND", CurrencyPositivePattern = 3 } }));
                            }

                            // Total de la facture
                            table.Cell().ColumnSpan(4).Element(TotalCellStyle).Text($"Total de la facture : {facture.MontantTotal.ToString("C", new CultureInfo("en-TN") { NumberFormat = { CurrencySymbol = " TND", CurrencyPositivePattern = 3 } })}")                                
                                .FontSize(14)
                                .Bold();

                            static IContainer TotalCellStyle(IContainer container)
                            {
                                return container
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .AlignRight()
                                    .AlignMiddle();
                            }
                        });
                    }
                });

                // Pied de page
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Merci pour votre confiance !").Bold().FontColor(Colors.Blue.Darken3);
                    text.Span(" - ").FontColor(Colors.Grey.Darken1);
                    text.Span("Contactez-nous pour toute question.").Italic().FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .AlignCenter()
            .AlignMiddle();
    }
}