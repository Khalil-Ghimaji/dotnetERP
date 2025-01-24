using Persistence.entities.Facturation;

namespace Facturation.Services;

public interface IPDFService
{
    byte[] GenererFacturePDF(Facture facture);
}
